locals {
  demo_confidential_permissions = ["viewer",  "writer"]
}

resource "keycloak_openid_client" "demo_confidential" {
  realm_id                 = "local"
  client_id                = "demo-confidential"
  access_type              = "CONFIDENTIAL"
  client_secret            = "secret"
  service_accounts_enabled = true
  enabled                  = true
  full_scope_allowed       = false
}


data "keycloak_openid_client_service_account_user" "demo_confidential_sa" {
  realm_id  = "local"
  client_id = keycloak_openid_client.demo_confidential.id
}

resource "keycloak_openid_client_service_account_role" "sa_assignments" {
  for_each = toset(local.demo_confidential_permissions)

  realm_id                = "local"
  service_account_user_id = data.keycloak_openid_client_service_account_user.demo_confidential_sa.id
  client_id               = keycloak_openid_client.apiservice.id
  role                    = each.value
}

resource "keycloak_generic_role_mapper" "client_scope_role_assignments" {
  for_each = toset(local.demo_confidential_permissions)

  realm_id  = "local"
  client_id = keycloak_openid_client.demo_confidential.id
  role_id   = keycloak_role.apiservice_roles[each.value].id
}