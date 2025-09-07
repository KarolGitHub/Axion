terraform {
  required_version = ">= 1.5.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = ">= 5.0"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

resource "aws_s3_bucket" "cdn_bucket" {
  bucket = var.bucket_name
}

resource "aws_s3_bucket_public_access_block" "this" {
  bucket                  = aws_s3_bucket.cdn_bucket.id
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

resource "aws_cloudfront_origin_access_control" "oac" {
  name                              = "${var.project}-oac"
  description                       = "OAC for ${var.project}"
  origin_access_control_origin_type = "s3"
  signing_behavior                  = "always"
  signing_protocol                  = "sigv4"
}

data "aws_iam_policy_document" "bucket_policy" {
  statement {
    actions = [
      "s3:GetObject"
    ]
    principals {
      type        = "Service"
      identifiers = ["cloudfront.amazonaws.com"]
    }
    resources = [
      "${aws_s3_bucket.cdn_bucket.arn}/*"
    ]
    condition {
      test     = "StringEquals"
      variable = "AWS:SourceArn"
      values   = [aws_cloudfront_distribution.cdn.arn]
    }
  }
}

resource "aws_s3_bucket_policy" "this" {
  bucket = aws_s3_bucket.cdn_bucket.id
  policy = data.aws_iam_policy_document.bucket_policy.json
}

resource "aws_cloudfront_distribution" "cdn" {
  enabled             = true
  comment             = "${var.project} CDN"
  default_root_object = "index.html"

  origin {
    domain_name              = aws_s3_bucket.cdn_bucket.bucket_regional_domain_name
    origin_id                = "s3-${aws_s3_bucket.cdn_bucket.id}"
    origin_access_control_id = aws_cloudfront_origin_access_control.oac.id
  }

  default_cache_behavior {
    allowed_methods  = ["GET", "HEAD", "OPTIONS"]
    cached_methods   = ["GET", "HEAD", "OPTIONS"]
    target_origin_id = "s3-${aws_s3_bucket.cdn_bucket.id}"

    viewer_protocol_policy = "redirect-to-https"

    compress = true

    forwarded_values {
      query_string = true
      headers      = ["Origin"]
      cookies {
        forward = "none"
      }
    }

    min_ttl     = 0
    default_ttl = var.default_ttl_seconds
    max_ttl     = var.max_ttl_seconds
  }

  price_class = var.price_class

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    cloudfront_default_certificate = true
  }
}

output "distribution_domain_name" {
  value       = aws_cloudfront_distribution.cdn.domain_name
  description = "CloudFront domain name to use as PUBLIC_URL/CDN_ORIGIN"
}

output "s3_bucket" {
  value = aws_s3_bucket.cdn_bucket.id
}

