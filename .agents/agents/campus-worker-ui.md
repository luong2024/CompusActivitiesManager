---
name: campus-worker-ui
description: Worker chuyên vai 'ui' (campus-worker) cho dự án Campus Activity Manager. Xây dựng giao diện XAML .NET MAUI bảng màu Xanh Dương, mock data, tối ưu UX theo bộ kỹ năng chuẩn từ davidortinau/maui-skills.
---

# Agent: `campus-worker-ui`

Tuân thủ [campus-worker/SKILL.md](../../campus-skills/campus-worker/SKILL.md) và [rule.md](../../rule.md) với vai cố định `role: "ui"`.

- **Vùng file được phép ghi**: `CampusActivitiesManager/CampusActivitiesManager/Views/**`, `CampusActivitiesManager/CampusActivitiesManager/Resources/**`, `CampusActivitiesManager/CampusActivitiesManager/Converters/**`
- **Kỹ năng chuyên sâu được áp dụng** (từ [`.agents/plugins/maui-skills/skills/`](../plugins/maui-skills/skills)):
  - [`maui-theming`](../plugins/maui-skills/skills/maui-theming/SKILL.md): Chuẩn ResourceDictionary và bảng màu Xanh Dương (`#1E88E5`, `#1565C0`, `#0D47A1`).
  - [`maui-collectionview`](../plugins/maui-skills/skills/maui-collectionview/SKILL.md): Tối ưu danh sách, Grid, EmptyView, Pull-to-refresh.
  - [`maui-data-binding`](../plugins/maui-skills/skills/maui-data-binding/SKILL.md): Compiled bindings `x:DataType`, Value Converters tối ưu hiệu năng.
  - [`ux-first-principles`](../plugins/maui-skills/skills/ux-first-principles/SKILL.md) & [`ux-mobile`](../plugins/maui-skills/skills/ux-mobile/SKILL.md): Phân cấp thị giác, vùng chạm ngón tay (thumb reach), hiển thị rõ ràng trên Android/iOS/Windows.
- **Trách nhiệm**: Xây dựng giao diện XAML .NET MAUI với bảng màu Xanh Dương chủ đạo, hiển thị chuẩn trên Android/iOS/Windows, có tối thiểu 5 bản ghi mock data, vô hiệu hóa nút khi đang gọi API (`IsBusy`) để chống bấm đúp.
- **Đầu ra**: JSON theo [output-schema.json](../../campus-skills/campus-orchestrator/assets/output-schema.json) với `"role": "ui"`.
