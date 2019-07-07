FROM debian:9.8-slim

# For deployment, we need
# libc6 - dynamically linked by cockroach binary
# ca-certificates - to authenticate TLS connections for telemetry and
#                   bulk-io with S3/GCS/Azure
# tzdata - for time zone functions
RUN apt-get update && \
	apt-get -y upgrade && \
	apt-get install -y libc6 ca-certificates tzdata wget && \
	rm -rf /var/lib/apt/lists/*

RUN mkdir -p /cockroach

RUN wget https://binaries.cockroachdb.com/cockroach-latest.linux-amd64.tgz
RUN tar -xvzf cockroach-latest.linux-amd64.tgz
RUN cp cockroach-latest.linux-amd64/cockroach /usr/local/bin/

ENTRYPOINT ["cockroach", "start", "--insecure"]
