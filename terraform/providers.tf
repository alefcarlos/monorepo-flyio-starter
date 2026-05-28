terraform {
  required_providers {
    keycloak = {
      source  = "keycloak/keycloak"
      version = "5.7.0"
    }
  }
}

provider "keycloak" {
  url       = "https://localhost:8080"
  client_id = "admin-cli"
  username  = "admin"
  password  = "admin"
}