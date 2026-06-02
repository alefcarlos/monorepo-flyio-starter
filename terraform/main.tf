resource "keycloak_realm" "local" {
  realm                 = "local"
  organizations_enabled = true

  login_with_email_allowed = true
  registration_email_as_username = true
}

resource "keycloak_organization" "acme" {
  realm   = keycloak_realm.local.id
  name    = "acme"
  alias   = "acme"
  enabled = true

  domain {
    name = "acme.com"
  }
}

resource "keycloak_organization" "bar" {
  realm   = keycloak_realm.local.id
  name    = "bar"
  alias   = "bar"
  enabled = true

  domain {
    name = "bar.com"
  }
}
