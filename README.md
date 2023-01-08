# Grip op Gras 2.0
A web application that supports dairy farmers in their daily management. The main purpose of this application is to generate  rations that meets the needs of dairy herds.

## Getting Started

### Prerequisites
- Visual Studio 2022
- Having the "ASP.NET and web development" workload installed
- Docker Desktop (for running the application with Prometheus and Grafana)

### Build & Run
1. Open BlazorWebAssemblyPrometheusExample.sln
2. Run the "docker-compose" project

### Links
- The Blazor application can be found at [http://localhost:4200/](http://localhost:4200/)
- Prometheus can be found at [http://localhost:9090/](http://localhost:9090/)
- Grafana can be found at [http://localhost:3000/](http://localhost:3000/)

### Setup Grafana
1. Open Grafana
2. Login into Grafana by using "admin" for both the username and password
3. Enter a new password
4. Go to Dashboards -> New -> Import
5. Select the dashboard that you want to import from [/grafana/provisioning/dashboards/](/grafana/provisioning/dashboards/)
6. Select "Prometheus" as the data source