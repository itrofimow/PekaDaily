use actix_web::{error::ErrorInternalServerError, web, App, Error, HttpServer};
use futures::future::{lazy, ok, Future};
use futures::stream::Stream;
use std::sync::Arc;
use tokio::runtime::current_thread::Runtime;
use tokio_postgres::SimpleQueryMessage::Row;
use tokio_postgres::{error::Error as DbError, Client, NoTls};

use bb8::Pool;
use bb8_postgres::PostgresConnectionManager;

struct PekaRepository {
    pool: Pool<PostgresConnectionManager<NoTls>>,
    connection_string: &'static str,
}

impl PekaRepository {
    fn new(pool: Pool<PostgresConnectionManager<NoTls>>, connection_string: &'static str) -> Self {
        PekaRepository {
            pool,
            connection_string,
        }
    }

    fn get_client(&self) -> impl Future<Item = Client, Error = DbError> {
        Box::new(
            tokio_postgres::connect(self.connection_string, NoTls)
                .map(|(client, connection)| {
                    let connection = connection.map_err(|e| {
                        eprintln!("db connection error! {}", e);
                    });
                    actix::spawn(connection);

                    client
                })
                .map_err(|e| {
                    eprintln!("fatal db connection error {}", e);
                    e
                }),
        )
    }

    fn test(&self) -> impl Future<Item = String, Error = DbError> {
        self.get_client()
            .and_then(|mut client| client.simple_query("select peka from peka;").collect())
            .then(|db_res| {
                let res = db_res.unwrap();
                let str = match &res[0] {
                    Row(row) => match row.get(0).take() {
                        None => panic!(),
                        Some(val) => val,
                    },
                    _ => panic!(),
                };
                ok::<String, DbError>(String::from(str))
            })
    }
}

fn get_current_peka(
    repo: web::Data<Arc<PekaRepository>>,
) -> Box<Future<Item = String, Error = Error>> {
    Box::new(repo.test().map_err(|e| {
        println!("error! {}", e);
        ErrorInternalServerError(e)
    }))
}

fn main() {
    let connection_manager =
        PostgresConnectionManager::new("postgresql://root@localhost:26257", NoTls);

    let mut rt = Runtime::new().unwrap();
    let pool = rt.block_on(lazy(|| {
        Pool::builder()
            .max_size(100)
            .build(connection_manager)
            .map_err(|e| panic!(e))
    })).unwrap();
        
    let peka_repo = Arc::new(PekaRepository::new(pool, "host=localhost port=26257 user=root"));
    let server = HttpServer::new(move || {
        App::new()
            .data(peka_repo.clone())
            .route("/api/current", web::get().to_async(get_current_peka))
    })
    .bind("0.0.0.0:1337")
    .unwrap();

    println!("listening on *:1337");
    
    server.run().unwrap();
}
