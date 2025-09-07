variable "project" {
  type        = string
  description = "Project name"
  default     = "axion"
}

variable "aws_region" {
  type        = string
  description = "AWS region"
  default     = "us-east-1"
}

variable "bucket_name" {
  type        = string
  description = "S3 bucket name for CDN artifacts"
}

variable "default_ttl_seconds" {
  type        = number
  description = "Default TTL"
  default     = 3600
}

variable "max_ttl_seconds" {
  type        = number
  description = "Max TTL"
  default     = 86400
}

variable "price_class" {
  type        = string
  description = "CloudFront price class"
  default     = "PriceClass_100"
}

