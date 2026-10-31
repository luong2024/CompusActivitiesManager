# Hệ thống Multi-Agent dự án Campus Activity Manager (`AGENTS.md`)

Tài liệu này định nghĩa hệ thống Agent cho dự án **Campus Activity Manager** (`.NET MAUI` + `ASP.NET Core Web API` tích hợp **Firebase**) dựa trên thư mục [`campus-skills/`](./campus-skills), bộ kỹ năng chuyên sâu [`.NET MAUI Skills (David Ortinau)`](./.agents/plugins/maui-skills/skills), và quy tắc dự án [`rule.md`](./rule.md).

---

## 1. Danh sách các Agent trong dự án

Dự án sử dụng mô hình 3 tầng Agent (**Orchestrator → Worker → Reviewer**) đã được đăng ký trong [`.agents/agents/`](./.agents/agents), [`.agents/skills.json`](./.agents/skills.json) và [`.agents/plugins.json`](./.agents/plugins.json):

| Tên Agent / Subagent | Vai trò | Kỹ năng & Thư mục sở hữu | Quyền hạn chính |
|---|---|---|---|
| **`campus-orchestrator`** | Điều phối Sprint / User Story (`USxx`/`Txx.x`), lập Task Plan & API Contract, giao việc qua Task Brief, tổng hợp Sprint Walkthrough | [`campus-skills/campus-orchestrator/SKILL.md`](./campus-skills/campus-orchestrator/SKILL.md) | Phân rã task, gọi subagent, tạo Artifact; **không** tự viết code hay test |
| **`campus-worker`** | Thực thi **1 task** (`Txx.x`) theo brief từ Orchestrator ở 1 trong 4 vai (`api`, `ui`, `integration`, `qa`) | [`campus-skills/campus-worker/SKILL.md`](./campus-skills/campus-worker/SKILL.md) | Chỉ sửa file trong vùng sở hữu của vai được giao, chạy build/test, xuất JSON theo `output-schema.json` |
| **`campus-worker-api`** | Worker chuyên vai `api` | `CampusActivitiesManager.Api/**` | Triển khai REST API, validation, Firebase Admin SDK & Realtime Database |
| **`campus-worker-ui`** | Worker chuyên vai `ui` | `CampusActivitiesManager/CampusActivitiesManager/Views/**`, `Resources/**`, `Converters/**` | XAML .NET MAUI bảng màu Xanh Dương, mock data, tối ưu UX theo `maui-theming`, `maui-data-binding`, `ux-first-principles` |
| **`campus-worker-integration`** | Worker chuyên vai `integration` | `Services/**`, `ViewModels/**`, `Models/**`, `MauiProgram.cs`, `AppShell.xaml*` | Tích hợp API theo `maui-rest-api`, `maui-secure-storage`, `maui-shell-navigation`, `maui-dependency-injection` |
| **`campus-worker-qa`** | Worker chuyên vai `qa` | `CampusActivitiesManager.Api.Tests/**` | Viết & chạy test theo `maui-unit-testing`, rà soát API lỗi thời với `maui-current-apis` |
| **`campus-reviewer`** | Kiểm tra độc lập đầu ra của Worker theo từng AC và DoD1–DoD6 trước khi merge | [`campus-skills/campus-reviewer/SKILL.md`](./campus-skills/campus-reviewer/SKILL.md) | Đọc diff, chạy lại test/build, kiểm tra bảo mật & vùng file; **không** sửa code hộ Worker |

---

## 2. Chuỗi phối hợp chuẩn giữa các Agent

```
API Contract → (campus-worker-api ‖ campus-worker-ui ‖ campus-worker-qa) → campus-worker-integration → campus-worker-qa → campus-reviewer
```

1. **Khi người dùng yêu cầu triển khai Sprint / User Story / Task** (ví dụ: *"chạy sprint 2"*, *"triển khai US07"*, *"chia việc T13.1"*):
   - Kích hoạt kỹ năng [`campus-orchestrator`](./campus-skills/campus-orchestrator/SKILL.md) hoặc gọi subagent `campus-orchestrator`.
   - Kiểm tra [`backlog-issues.md`](./campus-skills/campus-orchestrator/references/backlog-issues.md) và [`roles-and-ownership.md`](./campus-skills/campus-orchestrator/references/roles-and-ownership.md).
   - Soạn Task Brief theo [`task-brief-template.md`](./campus-skills/campus-orchestrator/assets/task-brief-template.md).
2. **Khi thực thi từng Task (`Txx.x`)**:
   - Giao cho `campus-worker` (hoặc `campus-worker-api`, `campus-worker-ui`, `campus-worker-integration`, `campus-worker-qa`).
   - Tận dụng các hướng dẫn chuẩn từ [`.agents/plugins/maui-skills/skills/`](./.agents/plugins/maui-skills/skills).
   - Worker trả kết quả theo đúng JSON Schema tại [`output-schema.json`](./campus-skills/campus-orchestrator/assets/output-schema.json).
3. **Khi Worker báo `done`**:
   - Chuyển kết quả sang `campus-reviewer` để đối chiếu từng AC, phạm vi file, bảo mật và **DoD1–DoD6**.
4. **Quy tắc bắt buộc chung**:
   - Mọi Agent phải tuân thủ đầy đủ [`rule.md`](./rule.md) (kiến trúc MVVM, bảng màu Xanh Dương chủ đạo, bảo mật Firebase Admin SDK ở tầng Server API, commit message `<TaskID>: <mô tả ngắn>`, và mẫu báo cáo Mục 12).

---

## 3. Bộ kỹ năng chuyên sâu .NET MAUI (`davidortinau/maui-skills`)

Dự án đã tích hợp và học hỏi toàn diện 41 kỹ năng từ Microsoft ([`davidortinau/maui-skills`](https://github.com/davidortinau/maui-skills)) tại [`.agents/plugins/maui-skills/skills/`](./.agents/plugins/maui-skills/skills):

| Lĩnh vực | Kỹ năng chính áp dụng | Vai trò áp dụng |
|---|---|---|
| **Giao diện & UX** | `maui-theming`, `maui-collectionview`, `maui-data-binding`, `ux-first-principles`, `ux-mobile`, `ux-desktop`, `maui-animations`, `maui-accessibility` | `campus-worker-ui` |
| **API & Tích hợp** | `maui-rest-api`, `maui-secure-storage`, `maui-shell-navigation`, `maui-dependency-injection`, `maui-authentication` | `campus-worker-integration` |
| **Kiểm thử & Tối ưu** | `maui-unit-testing`, `maui-current-apis`, `maui-performance`, `maui-hot-reload-diagnostics` | `campus-worker-qa`, `campus-reviewer` |
| **Dịch vụ nâng cao** | `maui-local-notifications`, `maui-push-notifications`, `maui-sqlite-database`, `maui-media-picker` | Khi triển khai thông báo sự kiện (US04/US16) |
