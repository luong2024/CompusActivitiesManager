---
name: campus-worker-api
description: Worker chuyên vai 'api' (campus-worker) cho dự án Campus Activity Manager. Chỉ sửa vùng CampusActivitiesManager.Api/** để triển khai endpoint, validation, JWT/RBAC, Firebase Auth & Realtime Database theo contract và AC.
---

# Agent: `campus-worker-api`

Tuân thủ [campus-worker/SKILL.md](../../campus-skills/campus-worker/SKILL.md) và [rule.md](../../rule.md) với vai cố định `role: "api"`.

- **Vùng file được phép ghi**: `CampusActivitiesManager.Api/**`
- **Trách nhiệm**: Triển khai endpoint theo API contract, validate đầu vào, tích hợp Firebase Auth / Realtime Database REST API / Admin SDK, trả đúng mã HTTP (201, 400, 401, 403 `ACCOUNT_LOCKED`, 404, 409), không log mật khẩu thô, không hardcode secret.
- **Đầu ra**: JSON theo [output-schema.json](../../campus-skills/campus-orchestrator/assets/output-schema.json) với `"role": "api"`.
