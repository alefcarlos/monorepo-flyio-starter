
resource "keycloak_user" "alice" {
  realm_id       = keycloak_realm.local.id
  username       = "alice@acme.com"
  enabled        = true
  email_verified = true

  email      = "alice@acme.com"
  first_name = "Alice"
  last_name  = "Aliceberg"

  initial_password {
    value = "123"
  }
}

resource "restful_resource" "alice_to_acme" {
  path = "/admin/realms/local/organizations/${keycloak_organization.acme.id}/members"

  body = keycloak_user.alice.id
}

resource "keycloak_user" "bob" {
  realm_id       = keycloak_realm.local.id
  username       = "bob@bar.com"
  enabled        = true
  email_verified = true

  email      = "bob@bar.com"
  first_name = "Bob"
  last_name  = "Fritz"

  initial_password {
    value = "123"
  }
}

resource "restful_resource" "bob_to_foo" {
  path = "/admin/realms/local/organizations/${keycloak_organization.bar.id}/members"

  body = keycloak_user.bob.id
}