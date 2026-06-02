locals {
  apiservice_roles = ["viewer", "writer"]
}

resource "keycloak_openid_client" "apiservice" {
  realm_id                 = keycloak_realm.local.id
  client_id                = "apiservice"
  access_type              = "BEARER-ONLY"
  enabled                  = true
}

resource "keycloak_role" "apiservice_roles" {
  for_each    = toset(local.apiservice_roles)
  
  realm_id    = keycloak_realm.local.id
  client_id   = keycloak_openid_client.apiservice.id
  name        = "${each.value}"
}