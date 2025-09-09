#!/bin/bash

# JWT Secret Generator
# Generates a cryptographically secure random string for JWT signing

# Default length (256 bits = 32 bytes = 64 hex characters)
LENGTH=${1:-64}

# Generate random bytes and convert to hex
JWT_SECRET=$(openssl rand -hex $((LENGTH / 2)))

echo "Generated JWT Secret:"
echo "$JWT_SECRET"