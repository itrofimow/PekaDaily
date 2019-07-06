use actix_web::{web, Error, error::ErrorInternalServerError};
use futures::future::{ok, Future};
use futures::stream::Stream;
use tokio_postgres::{error::Error as DbError, Client, NoTls};

#[derive(Clone)]
struct PekaRepository {
    connection_string: &'static str,
}

impl PekaRepository {
    fn new(connection_string: &'static str) -> Self {
        PekaRepository { connection_string }
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
            .and_then(|mut client| {
                client.simple_query("SHOW DATABASES;").collect()
            })
            .then(|_| ok::<String, DbError>(String::from("asdasd")))
    }
}

fn get_current_peka(repo: web::Data<PekaRepository>) -> Box<Future<Item = String, Error = Error>> {
    Box::new(
        repo.test()
        .map_err(|e| {
            println!("error! {}", e);
            ErrorInternalServerError(e)
        }))
}

fn main() {
    let peka_repo = PekaRepository::new("host=localhost port=26257 user=root");

    use actix_web::{App, HttpServer};
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
