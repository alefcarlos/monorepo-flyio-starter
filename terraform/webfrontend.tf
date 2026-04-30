resource "keycloak_openid_client" "webfrontend" {
    realm_id                                       = data.keycloak_realm.local.id
    access_type                                    = "PUBLIC"
    client_authenticator_type                      = "client-secret"
    client_id                                      = "webfrontend"
    pkce_code_challenge_method                     = "S256"
    standard_flow_enabled                          = true
    valid_post_logout_redirect_uris                = [
        "https://webfrontend-apphost.dev.localhost:7223/signout-callback-oidc",
    ]
    valid_redirect_uris                            = [
        "https://webfrontend-apphost.dev.localhost:7223/signin-oidc",
    ]
    web_origins                                    = [
        "https://webfrontend-apphost.dev.localhost:7223",
    ]
}