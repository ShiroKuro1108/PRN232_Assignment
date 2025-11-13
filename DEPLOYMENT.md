Deployment and Security Checklist

This file contains recommended steps to deploy the application safely and avoid leaking secrets.

1) Secrets and environment variables
- Never commit secrets (Stripe secret keys, DB passwords, JWT secret) to git.
- Use environment variables on your host (Render, Vercel, Heroku, Docker secrets, Kubernetes secrets).
- Required env vars:
  - Stripe__SecretKey (server) - REQUIRED
  - JwtSettings__SecretKey (server) - REQUIRED
  - DATABASE_URL or DefaultConnection (server) - REQUIRED
  - REACT_APP_API_URL (client) - optional for dev
  - REACT_APP_STRIPE_PUBLIC_KEY (client) - publishable key for client-side Stripe (safe to expose)

2) Publishable vs Secret Stripe keys
- Publishable key (pk_...) is intended for client usage (Stripe Elements). It is safe to include in front-end bundles.
- Secret key (sk_...) must never be exposed in client code or committed. Keep it server-side only and provide it via an environment variable.

3) Webhooks
- Configure Stripe webhooks in your production Stripe dashboard to point at /api/payment/webhook on your deployed host.
- Set the webhook signing secret as an environment variable (e.g. STRIPE_WEBHOOK_SECRET) and verify signatures in the webhook handler.

4) Key rotation
- Have a plan to rotate Stripe keys: create a new key in Stripe dashboard and update the environment variable in your host. For zero-downtime, add support for reading multiple keys or deploy quickly.

5) Removing secrets from git history (if needed)
- If you accidentally committed secrets, rotate them immediately in the provider (Stripe, DB password).
- To scrub git history, use the BFG Repo Cleaner or git-filter-repo. Example (local):

  # Using git-filter-repo (recommended over filter-branch)
  git clone --mirror <repo-url>
  cd repo.git
  git filter-repo --replace-text replacements.txt
  # push back to origin (force)

  # Or use BFG: https://rtyley.github.io/bfg-repo-cleaner/

6) CI/CD and secret storage
- Use your cloud provider's secret manager (Render's env vars, Vercel secrets, GitHub Actions secrets) and avoid storing secrets in CI logs.

7) TLS and CORS
- Ensure TLS (HTTPS) is enabled in production endpoints.
- Lock down CORS origins to your production front-end domains.

8) Minimal checklist before going live
- [ ] Stripe__SecretKey set in production env
- [ ] JwtSettings__SecretKey set in production env
- [ ] Database connection configured securely (no public credentials in source)
- [ ] Webhook endpoint configured and signing secret stored
- [ ] No dev-only hardcoded secrets in source (run grep for sk_ and pk_)

If you want, I can also:
- Add verification for the webhook signing secret in `PaymentController` if it's not already implemented.
- Create simple deployment instructions for Render / Heroku / Docker.

