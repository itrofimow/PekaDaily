FROM rust:1.36 as builder

WORKDIR src
COPY . .

RUN cargo build --release

ENTRYPOINT ["./target/release/api"]

