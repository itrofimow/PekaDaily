use actix_web::{error::ErrorInternalServerError, web, App, Error, HttpServer};
use futures::future::{lazy, ok, err, Future};
use futures::stream::Stream;
use std::sync::Arc;
use tokio::runtime::current_thread::Runtime;
use tokio_postgres::SimpleQueryMessage::Row;
use tokio_postgres::{error::Error as DbError, Client, NoTls};

use bb8::Pool;
use bb8_postgres::PostgresConnectionManager;

struct PekaRepository {
    pool: Pool<PostgresConnectionManager<NoTls>>
}

impl PekaRepository {
    fn new(pool: Pool<PostgresConnectionManager<NoTls>>) -> Self {
        PekaRepository {
            pool
        }
    }

    fn get_current_peka(&self) -> impl Future<Item = String, Error = DbError> {
        self.pool.run(|mut connection: Client| {
            connection.simple_query("select peka from peka;").collect().then(move |r| {
                match r {
                    Ok(res) => match &res[0] {
                        Row(row) => match row.get(0).take() {
                            None => panic!(),
                            Some(val) => ok((String::from(val), connection))
                        },
                        _ => panic!()
                    },
                    Err(e) => err((e, connection))
                }
            })
        })
        .map_err(|_| panic!())
    }
}

fn get_current_peka(
    repo: web::Data<Arc<PekaRepository>>,
) -> Box<Future<Item = String, Error = Error>> {
    Box::new(repo.get_current_peka().map_err(|e| {
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
        
    let peka_repo = Arc::new(PekaRepository::new(pool));
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
