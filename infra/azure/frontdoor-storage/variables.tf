variable "project" {
  type        = string
  description = "Project name"
  default     = "axion"
}

variable "location" {
  type        = string
  description = "Azure region"
  default     = "westeurope"
}

variable "storage_account_name" {
  type        = string
  description = "Globally unique storage account name"
}

