terraform {
  required_version = ">= 1.5.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 3.100.0"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "${var.project}-rg"
  location = var.location
}

resource "azurerm_storage_account" "storage" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  static_website {
    index_document     = "index.html"
    error_404_document = "index.html"
  }
}

resource "azurerm_cdn_frontdoor_profile" "fd_profile" {
  name                = "${var.project}-fd-profile"
  resource_group_name = azurerm_resource_group.rg.name
  sku_name            = "Standard_AzureFrontDoor"
}

resource "azurerm_cdn_frontdoor_endpoint" "fd_endpoint" {
  name                     = "${var.project}-fd-endpoint"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.fd_profile.id
}

resource "azurerm_cdn_frontdoor_origin_group" "fd_origin_group" {
  name                     = "${var.project}-og"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.fd_profile.id
  load_balancing {
    sample_size                 = 4
    successful_samples_required = 3
  }
}

resource "azurerm_cdn_frontdoor_origin" "fd_origin" {
  name                          = "${var.project}-origin"
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.fd_origin_group.id
  enabled                       = true
  host_name                     = azurerm_storage_account.storage.primary_web_host
  http_port                     = 80
  https_port                    = 443
  origin_host_header            = azurerm_storage_account.storage.primary_web_host
  priority                      = 1
  weight                        = 1000
}

resource "azurerm_cdn_frontdoor_route" "fd_route" {
  name                          = "${var.project}-route"
  cdn_frontdoor_endpoint_id     = azurerm_cdn_frontdoor_endpoint.fd_endpoint.id
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.fd_origin_group.id
  supported_protocols           = ["Http", "Https"]
  https_redirect_enabled        = true
  patterns_to_match             = ["/*"]
  forwarding_protocol           = "HttpsOnly"
  caching {
    query_string_caching_behavior = "IgnoreQueryString"
  }
}

output "cdn_hostname" {
  value       = azurerm_cdn_frontdoor_endpoint.fd_endpoint.host_name
  description = "Front Door hostname to use as PUBLIC_URL/CDN_ORIGIN"
}

output "static_website_url" {
  value = azurerm_storage_account.storage.primary_web_endpoint
}

