FROM rust:1.36 as builder
WORKDIR build
# We'll get to what this file is below!
COPY dummy.rs /build
# If this changed likely the Cargo.toml changed so lets trigger the
# recopying of it anyways
COPY Cargo.lock /build
COPY Cargo.toml /build
# We'll get to what this substitution is for but replace main.rs with
# lib.rs if this is a library
RUN cat Cargo.toml
RUN sed -i 's|src/main.rs|dummy.rs|g' Cargo.toml
RUN cat Cargo.toml
# Drop release if you want debug builds. This step cache's our deps!
RUN cargo build --release
# Now return the file back to normal
RUN sed -i 's|dummy.rs|src/main.rs|g' Cargo.toml
# Copy the rest of the files into the container
COPY . /build
# Now this only builds our changes to things like src
RUN cargo build --release

FROM rust:1.36-slim as runtime
WORKDIR app
COPY --from=builder build/target/release/api .
ENTRYPOINT ["./api"]
