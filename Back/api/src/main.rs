use mobc::runtime::DefaultExecutor;
use mobc::Error;
use mobc::Pool;
use mobc_postgres::PostgresConnectionManager;
use std::str::FromStr;
use std::sync::Arc;
use tokio_postgres::Config;
use tokio_postgres::Error as PostgresError;
use tokio_postgres::NoTls;
use actix_web::{HttpServer, App, web, Responder, get};

type CockroachPool = Pool<PostgresConnectionManager<NoTls, DefaultExecutor>>;

async fn build_pool() -> Result<CockroachPool, Error<PostgresError>> {
    let config = Config::from_str("postgresql://root@localhost:26257")?;
    let manager = PostgresConnectionManager::new(config, NoTls);

    Ok(Pool::builder().max_size(200).build(manager).await?)
}

async fn get_current_peka(pool: Arc<CockroachPool>) -> Result<String, Error<PostgresError>> {
	let conn = pool.get().await?;
	let r = conn.query("select peka from peka", &[]).await?;
	let value: &str = r[0].get(0);

	Ok(String::from(value))
}

#[get("/api/current")]
async fn current_peka(pool: web::Data<CockroachPool>) -> impl Responder {
	let result = get_current_peka(pool.into_inner()).await;
	match result {
		Ok(data) => data,
		Err(_) => String::from("internal server error")
	}
}

#[actix_rt::main]
async fn main() -> std::io::Result<()> {
    let pool = build_pool().await.unwrap();

	HttpServer::new(move || {
		App::new().data(pool.clone()).service(current_peka)
	})
	.bind("0.0.0.0:1337")?
	.start()
	.await
}
