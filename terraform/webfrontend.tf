resource "keycloak_openid_client" "webfrontend" {
  realm_id                   = keycloak_realm.local.id
  access_type                = "PUBLIC"
  client_authenticator_type  = "client-secret"
  client_id                  = "webfrontend"
  pkce_code_challenge_method = "S256"
  standard_flow_enabled      = true
  full_scope_allowed         = false
  valid_post_logout_redirect_uris = [
    "https://webfrontend-apphost.dev.localhost:7223/signout-callback-oidc",
  ]
  valid_redirect_uris = [
    "https://webfrontend-apphost.dev.localhost:7223/signin-oidc",
  ]
  web_origins = [
    "https://webfrontend-apphost.dev.localhost:7223",
  ]
}

resource "keycloak_openid_audience_protocol_mapper" "audience_mapper" {
  realm_id  = keycloak_realm.local.id
  client_id = keycloak_openid_client.webfrontend.id
  name      = "audience-mapper"

  included_custom_audience = "apiservice"
}