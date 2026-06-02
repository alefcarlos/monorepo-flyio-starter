terraform {
  required_providers {
    keycloak = {
      source  = "keycloak/keycloak"
      version = "5.7.0"
    }

    restful = {
      source  = "magodo/restful"
      version = "0.25.2"
    }
  }
}

provider "keycloak" {
  url       = "https://localhost:8080"
  client_id = "admin-cli"
  username  = "admin"
  password  = "admin"
}


provider "restful" {
  base_url = "https://127.0.0.1:8080"
  security = {
    oauth2 = {
      client_credentials = {
        client_id     = "admin-cli"
        client_secret = "admin-cli"
        token_url     = "https://127.0.0.1:8080/realms/master/protocol/openid-connect/token"
        scopes        = ["openid"]
        endpoint_params = {
          username   = ["admin"]
          password   = ["admin"]
          grant_type = ["password"]
        }
      }
    }
  }
}