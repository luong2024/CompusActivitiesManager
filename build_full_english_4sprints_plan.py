import sys, openpyxl
from copy import copy

sys.stdout.reconfigure(encoding='utf-8')

base_path = r'C:\Users\Admin\Downloads\Project_Plan_Campus_Activity_Manager__MAUI__.xlsx'
wb = openpyxl.load_workbook(base_path)

def safe_set(ws, r, c, val):
    cell = ws.cell(r, c)
    if type(cell).__name__ != 'MergedCell':
        cell.value = val

# ==========================================
# 1. SHEET: Kế hoạch dự án (Project Plan)
# ==========================================
ws_kh = wb['Kế hoạch dự án']
safe_set(ws_kh, 1, 1, 'PROJECT PLAN')
safe_set(ws_kh, 2, 1, 'Practical Software Development Process - Agile / Scrum Framework')
safe_set(ws_kh, 3, 1, 'Note: Yellow cells contain project input data. Formula cells calculate totals automatically.')

safe_set(ws_kh, 5, 1, '1. General Project Information')
safe_set(ws_kh, 6, 2, 'Project / Product Name')
safe_set(ws_kh, 6, 4, 'Campus Activity Manager - Mobile App & Serverless Cloud Services (.NET MAUI & Firebase)')
safe_set(ws_kh, 7, 2, 'Client / Target Institution')
safe_set(ws_kh, 7, 4, 'Information and Communication Technology University (ICTU) - Thai Nguyen University')
safe_set(ws_kh, 8, 2, 'Start Date')
safe_set(ws_kh, 8, 4, '01/09/2026')
safe_set(ws_kh, 8, 6, 'End Date')
safe_set(ws_kh, 8, 7, '30/11/2026')
safe_set(ws_kh, 9, 2, 'Date of Plan')
safe_set(ws_kh, 9, 4, '01/09/2026')
safe_set(ws_kh, 9, 6, 'Version')
safe_set(ws_kh, 9, 7, 1.0)
safe_set(ws_kh, 10, 2, 'Project Objectives')
safe_set(ws_kh, 10, 4, ('Develop a modern, cross-platform mobile application (.NET MAUI 9.0) powered by a Serverless Cloud-Native '
                        'architecture directly integrating Google Cloud Firestore REST API and Firebase Authentication. The system empowers '
                        'ICTU students to explore upcoming campus activities, register for events, check in seamlessly via device camera QR scanner '
                        '(ZXing.Net.Maui.Controls) or photo proof (MediaPicker), track their accumulated extra-curricular training points (ĐRL) ledger, '
                        'and provides event organizers with real-time attendee monitoring and UTF-8 CSV report export with BOM.'))
safe_set(ws_kh, 11, 2, 'Project Scope')
safe_set(ws_kh, 11, 4, ('The project encompasses the end-to-end event and activity management lifecycle across 4 agile sprints:\n'
                        '• Sprint 1: Core Authentication, PBKDF2-SHA256 password security, SecureStorage session management, and Admin User Management.\n'
                        '• Sprint 2: Event Lifecycle Management, Category Management, and Tag-based Filtering.\n'
                        '• Sprint 3: Hardware Camera QR Scanning (ZXing), Photo Check-in (MediaPicker), Manual Fallback, and 2-tier Confirmation Modal.\n'
                        '• Sprint 4: Real-time Attendance Monitoring Dashboard, Training Points (ĐRL) Ledger, and UTF-8 CSV Export with BOM.'))
safe_set(ws_kh, 12, 2, 'Project Scale')
safe_set(ws_kh, 12, 4, 'Deployed for all full-time undergraduate students, student clubs, lecturers, and Youth Union administrators at ICTU.')
safe_set(ws_kh, 13, 2, 'Prepared by')
safe_set(ws_kh, 13, 4, 'Nguyen Duc Manh')
safe_set(ws_kh, 13, 6, 'Reviewed by')
safe_set(ws_kh, 13, 7, 'Defense Committee & Academic Advisors')
safe_set(ws_kh, 14, 2, 'Approved by')
safe_set(ws_kh, 14, 4, 'Nguyen Duc Manh')

# 2. Team Organization
safe_set(ws_kh, 16, 1, '2. Project Organization')
safe_set(ws_kh, 17, 2, '2.1. Scrum Project Team')
safe_set(ws_kh, 18, 2, 'No.')
safe_set(ws_kh, 18, 3, 'Full Name')
safe_set(ws_kh, 18, 5, 'Project Role')
safe_set(ws_kh, 18, 6, 'Responsibilities & Technical Domain')

if 'F21:F25' in ws_kh.merged_cells:
    ws_kh.unmerge_cells('F21:F25')
if 'G21:G25' in ws_kh.merged_cells:
    ws_kh.unmerge_cells('G21:G25')

team_members = [
    (19, 1.0, 'Nguyen Duc Manh', 'Product Owner & Tech Lead', 'Define product vision, prioritize Product Backlog, architect Serverless Cloud Firestore backend, oversee technical delivery.'),
    (20, 2.0, 'Tran Gia Khanh', 'Scrum Master & Integration Dev', 'Facilitate sprints, integrate AuthenticationService, SecureStorage, PBKDF2-SHA256 encryption, and Shell navigation.'),
    (21, 3.0, 'Nguyen An Cuong', 'Frontend / UI Developer', 'Build .NET MAUI XAML user interfaces using Blue Theme (#2563EB), optimize mobile UX/UI, and manage CollectionViews.'),
    (22, 4.0, 'Nguyen Van An', 'Mobile Feature Developer', 'Develop feature forms, implement Role-Based Access Control (RBAC), event lifecycle management, and category forms.'),
    (23, 5.0, 'Vu Tien Dat', 'QA & Test Engineer', 'Design comprehensive test plans, write Unit Tests & Integration Tests, and verify Definition of Done (DoD1–DoD6) compliance.'),
]
for r, stt, name, pos, role in team_members:
    safe_set(ws_kh, r, 2, stt)
    safe_set(ws_kh, r, 3, name)
    safe_set(ws_kh, r, 5, pos)
    safe_set(ws_kh, r, 6, role)

for r in range(24, 26):
    safe_set(ws_kh, r, 2, None)
    safe_set(ws_kh, r, 3, None)
    safe_set(ws_kh, r, 5, None)
    safe_set(ws_kh, r, 6, None)

# Stakeholders
safe_set(ws_kh, 26, 2, '2.2. External Stakeholders')
safe_set(ws_kh, 27, 2, 'No.')
safe_set(ws_kh, 27, 3, 'Stakeholder')
safe_set(ws_kh, 27, 5, 'Collaboration Scope')
safe_set(ws_kh, 27, 6, 'Focal Point')

stakeholders = [
    (28, 1.0, 'ICTU Youth Union & Student Association', 'Provide activity guidelines and extra-curricular training point (ĐRL) evaluation framework', 'Youth Union Representative'),
    (29, 2.0, 'ICTU IT Infrastructure Department', 'Provide campus networking access, student identity specifications, and deployment support', 'IT Staff Representative'),
    (30, 3.0, 'Student Clubs & Student Body', 'Provide usability feedback, participate in beta testing, and validate attendance workflows', 'Club Presidents & Students'),
]
for r, stt, name, cont, head in stakeholders:
    safe_set(ws_kh, r, 2, stt)
    safe_set(ws_kh, r, 3, name)
    safe_set(ws_kh, r, 5, cont)
    safe_set(ws_kh, r, 6, head)

# WBS / Sprints
safe_set(ws_kh, 46, 2, '4.1. Work Breakdown Structure (WBS) & Sprint Schedule')
wbs_data = [
    (47, 1.0, 'Project Initiation & Comprehensive Planning', 'Project Charter, Master Project Plan', 'Nguyen Duc Manh', 8.0, 'Week 1', 'Week 1'),
    (48, 2.0, 'Requirement Analysis & Product Backlog Specification (38 USs)', 'Prioritized Product Backlog, User Stories, AC1-AC4', 'Scrum Team', 20.0, 'Week 1', 'Week 2'),
    (49, 3.0, 'Serverless Architecture, Firestore Schema & MAUI UI Design', 'System Architecture, Firestore Collections, Wireframe, Blue Theme', 'Nguyen Duc Manh, Nguyen An Cuong', 20.0, 'Week 2', 'Week 3'),
    (50, 4.0, 'Sprint 1 & Sprint 2 Planning and Story Point Estimation', 'Sprint Backlogs, Sprint Goals, Task Breakdown', 'Scrum Team', 8.0, 'Week 3', 'Week 3'),
    (51, 5.0, 'Sprint 01 Development: Core Auth, Security & User Management', 'Increment 1 (AuthService, SecureStorage, PBKDF2, UserManagement)', 'Scrum Team', 79.0, 'Week 3', 'Week 4'),
    (52, 6.0, 'Sprint 01 Review & Retrospective', 'Accepted Increment 1, Updated Product Backlog', 'Scrum Team', 6.0, 'End Week 4', 'End Week 4'),
    (53, 7.0, 'Sprint 02 Development: Event Lifecycle & Category Management', 'Increment 2 (EventService, CategoryService, Tag Filter, Detail UI)', 'Scrum Team', 60.0, 'Week 5', 'Week 6'),
    (54, 8.0, 'Sprint 02 Review & Retrospective', 'Accepted Increment 2, Sprint 3 Preparation', 'Scrum Team', 6.0, 'End Week 6', 'End Week 6'),
    (55, 9.0, 'Sprint 03 Development: QR Scanner & Photo Proof Attendance', 'Increment 3 (ZXing QR Scanner, MediaPicker Photo Proof, Modal)', 'Scrum Team', 50.0, 'Week 7', 'Week 8'),
    (56, 10.0, 'Sprint 03 Review & Retrospective', 'Accepted Increment 3, High-Tech Verification', 'Scrum Team', 6.0, 'End Week 8', 'End Week 8'),
    (57, 11.0, 'Sprint 04 Development: Real-time Monitoring, ĐRL & CSV Export', 'Increment 4 (Live Attendee Monitor, ĐRL Ledger, UTF-8 BOM CSV)', 'Scrum Team', 45.0, 'Week 9', 'Week 10'),
    (58, 12.0, 'Integration Testing, Performance Profiling & Optimization', 'Release Candidate Package (.apk), Automated Test Suite', 'Vu Tien Dat & Team', 20.0, 'Week 11', 'Week 11'),
    (59, 13.0, 'Final Acceptance, Signed APK Packaging & Defense Documentation', 'Signed Production APK, User Manual, Final Defense Slides', 'Scrum Team', 10.0, 'Week 12', 'Week 12'),
]
for r, stt, act, prod, dev, hrs, start_w, end_w in wbs_data:
    safe_set(ws_kh, r, 2, stt)
    safe_set(ws_kh, r, 3, act)
    safe_set(ws_kh, r, 5, prod)
    safe_set(ws_kh, r, 6, dev)
    safe_set(ws_kh, r, 7, hrs)
    safe_set(ws_kh, r, 8, start_w)
    safe_set(ws_kh, r, 9, end_w)

# Deliverables
safe_set(ws_kh, 62, 2, '4.2. Project Deliverables')
deliverables = [
    (64, 1.0, 'Project Charter & Scope Statement', 'Week 1', 'Advisors / Committee', 'Post Kick-off'),
    (65, 2.0, 'Prioritized Product Backlog (38 User Stories)', 'Week 2', 'Advisors / Committee', 'Version 1.0'),
    (66, 3.0, 'Serverless Architecture, Firestore Schema & UI Wireframes', 'Week 3', 'Advisors / Committee', 'Design Complete'),
    (67, 4.0, 'Sprint 1 Increment: Authentication & User Management', 'End Week 4', 'Advisors / Committee', 'Sprint 1 Goal Met'),
    (68, 5.0, 'Sprint 2 Increment: Event Lifecycle & Category System', 'End Week 6', 'Advisors / Committee', 'Sprint 2 Goal Met'),
    (69, 6.0, 'Sprint 3 Increment: Hardware QR Scanner & Photo Proof Check-in', 'End Week 8', 'Advisors / Committee', 'Sprint 3 Goal Met'),
    (70, 7.0, 'Sprint 4 Increment: Realtime Monitoring, ĐRL Ledger & CSV Export', 'End Week 10', 'Advisors / Committee', 'All USs Completed'),
    (71, 8.0, 'Complete Source Code Repository (.NET MAUI 9.0 & Firebase)', 'Week 12', 'Faculty / Committee', 'GitHub Repository'),
    (72, 9.0, 'Compiled Mobile Application Package (CampusActivityManager.apk)', 'Week 12', 'Faculty / Committee', 'Android APK Package'),
    (73, 10.0, 'User Manual, Architecture Document & Defense Presentation', 'Week 12', 'Faculty / Committee', 'Final Defense'),
]
for r, stt, prod, date, place, note in deliverables:
    safe_set(ws_kh, r, 2, stt)
    safe_set(ws_kh, r, 3, prod)
    safe_set(ws_kh, r, 5, date)
    safe_set(ws_kh, r, 6, place)
    safe_set(ws_kh, r, 7, note)

print("Kế hoạch dự án (English) updated successfully.")

# ==========================================
# 2. SHEET: Release Plan & Burndown
# ==========================================
ws_rp = wb['Release Plan & Burndown']
safe_set(ws_rp, 1, 1, 'RELEASE PLAN & PRODUCT BURNDOWN')
safe_set(ws_rp, 3, 1, 'Step 1 — Input project baseline numbers:')
safe_set(ws_rp, 4, 2, 'Total Product Backlog Points (from A0)')
ws_rp['C4'] = 182.0
safe_set(ws_rp, 5, 2, 'Estimated Velocity (Points / Sprint)')
ws_rp['C5'] = 45.5
safe_set(ws_rp, 6, 2, 'Estimated Sprints to Complete')
safe_set(ws_rp, 7, 1, 'Note: 4 Sprints planned to deliver all 38 User Stories with full verification.')
safe_set(ws_rp, 8, 1, 'Step 2 — Product Burndown Table (Planned vs Actual points remaining):')
safe_set(ws_rp, 9, 1, 'Sprint')
safe_set(ws_rp, 9, 2, 'Points Remaining (Planned)')
safe_set(ws_rp, 9, 3, 'Points Remaining (Actual)')

ws_rp['C10'] = 182.0 # Sprint 0
ws_rp['C11'] = 136.0 # Sprint 1
ws_rp['C12'] = 91.0  # Sprint 2
ws_rp['C13'] = 45.0  # Sprint 3
ws_rp['C14'] = 0.0   # Sprint 4

for r in range(15, 21):
    safe_set(ws_rp, r, 3, None)

print("Release Plan & Burndown (English) updated successfully.")

# ==========================================
# 3. SHEET: Theo dõi tiến độ (tham khảo) - Sprint 1
# ==========================================
ws_td = wb['Theo dõi tiến độ (tham khảo)']
safe_set(ws_td, 1, 1, 'DETAILED SPRINT 1 TASK PLAN & PROGRESS TRACKING')
safe_set(ws_td, 4, 2, 'Sprint Goal:')
safe_set(ws_td, 4, 3, 'Build Serverless architecture foundation, user authentication, secure session storage, and user management.')
safe_set(ws_td, 5, 2, 'Duration:')
safe_set(ws_td, 5, 4, '10 working days (01/09 - 14/09)')
safe_set(ws_td, 6, 2, 'NET Capacity:')
ws_td['D6'] = 80
safe_set(ws_td, 7, 2, 'Story Points:')
ws_td['D7'] = 27.0
safe_set(ws_td, 8, 2, 'Sprint Status:')
safe_set(ws_td, 8, 4, 'Done')
safe_set(ws_td, 9, 2, 'Dev Team Size:')
ws_td['D9'] = 5.0

s1_detailed_tasks_en = [
    ('T05.1', 'US05', 'Develop User Management UI (UserManagementPage - MAUI)', 'Nguyen An Cuong', 3.0, 'High', 'Done', '01/09/2026', '02/09/2026', 3.0, 'Completed on time'),
    ('T05.2', 'US05', 'Implement Role-Based Access Control logic (Admin/Manager/Student) on Firestore', 'Nguyen Van An', 4.0, 'High', 'Done', '02/09/2026', '03/09/2026', 4.0, 'Completed on time'),
    ('T05.3', 'US05', 'Connect User Management UI with UserService (Cloud Firestore)', 'Tran Gia Khanh', 3.0, 'High', 'Done', '03/09/2026', '04/09/2026', 3.0, 'Completed on time'),
    ('T05.4', 'US05', 'Test User Management functionality (create, edit, lock, RBAC)', 'Vu Tien Dat', 3.0, 'Medium', 'Done', '04/09/2026', '05/09/2026', 3.0, 'Completed on time'),
    ('T35.1', 'US35', 'Build UserService for CRUD account operations directly on Cloud Firestore', 'Nguyen Duc Manh', 10.0, 'High', 'Done', '01/09/2026', '04/09/2026', 10.0, 'Completed on time'),
    ('T35.2', 'US35', 'Implement Lock/Unlock Account logic & integration test with login', 'Nguyen Van An', 8.0, 'High', 'Done', '04/09/2026', '06/09/2026', 8.0, 'Completed on time'),
    ('T26.1', 'US26', 'Develop Registration UI (RegisterPage - MAUI)', 'Nguyen An Cuong', 2.0, 'High', 'Done', '06/09/2026', '07/09/2026', 2.0, 'Completed on time'),
    ('T26.3', 'US26', 'Validate client-side registration inputs (email regex, password strength)', 'Tran Gia Khanh', 2.0, 'Medium', 'Done', '07/09/2026', '07/09/2026', 2.0, 'Completed on time'),
    ('T26.4', 'US26', 'Connect Registration UI with AuthenticationService & error handling', 'Tran Gia Khanh', 2.0, 'High', 'Done', '07/09/2026', '08/09/2026', 2.0, 'Completed on time'),
    ('T26.5', 'US26', 'Test registration workflows (Unit tests & Manual tests)', 'Vu Tien Dat', 2.0, 'Medium', 'Done', '08/09/2026', '08/09/2026', 2.0, 'Completed on time'),
    ('T36.1', 'US36', 'Implement serverless account registration with PBKDF2-SHA256 password hashing', 'Nguyen An Cuong', 10.0, 'High', 'Done', '06/09/2026', '09/09/2026', 10.0, 'Completed on time'),
    ('T28.1', 'US28', 'Develop Login UI (LoginPage - MAUI) with password visibility toggle', 'Nguyen An Cuong', 2.0, 'High', 'Done', '09/09/2026', '10/09/2026', 2.0, 'Completed on time'),
    ('T28.3', 'US28', 'Store session tokens and user credentials securely in SecureStorage', 'Tran Gia Khanh', 2.0, 'High', 'Done', '10/09/2026', '10/09/2026', 2.0, 'Completed on time'),
    ('T28.4', 'US28', 'Connect Login UI with AuthenticationService & Shell navigation by role', 'Nguyen Duc Manh', 1.0, 'High', 'Done', '10/09/2026', '11/09/2026', 1.0, 'Completed on time'),
    ('T28.5', 'US28', 'Test login scenarios (valid credentials, wrong password, locked account)', 'Vu Tien Dat', 2.0, 'Medium', 'Done', '11/09/2026', '11/09/2026', 2.0, 'Completed on time'),
    ('T37.1', 'US37', 'Build AuthenticationService with PBKDF2 verification and token generation', 'Tran Gia Khanh', 9.0, 'High', 'Done', '09/09/2026', '12/09/2026', 9.0, 'Completed on time'),
    ('T15.1', 'US15', 'Develop Category Tag UI on event cards (Seminar, Volunteer, Sports...)', 'Nguyen An Cuong', 1.0, 'Medium', 'Done', '12/09/2026', '13/09/2026', 1.0, 'Completed on time'),
    ('T15.3', 'US15', 'Integrate Category Tag display with home event list & testing', 'Vu Tien Dat', 1.0, 'Medium', 'Done', '13/09/2026', '13/09/2026', 1.0, 'Completed on time'),
    ('T38.1', 'US38', 'Build EventService query to fetch events by category from Cloud Firestore', 'Vu Tien Dat', 12.0, 'High', 'Done', '12/09/2026', '14/09/2026', 12.0, 'Completed on time'),
    ('T30.1', 'US30', 'Implement secure logout and clean up SecureStorage token session', 'Tran Gia Khanh', 2.0, 'High', 'Done', '13/09/2026', '13/09/2026', 2.0, 'Completed on time'),
    ('T27.1', 'US27', 'Develop Forgot Password UI and email reset request flow', 'Nguyen An Cuong', 3.0, 'Medium', 'Done', '13/09/2026', '14/09/2026', 3.0, 'Completed on time'),
    ('T35.3', 'US35', 'Optimize user list pagination and multi-criteria search on Firestore', 'Nguyen Duc Manh', 4.0, 'High', 'Done', '14/09/2026', '14/09/2026', 4.0, 'Completed on time'),
    ('T28.6', 'US28', 'Implement auto-login session restoration upon mobile app startup', 'Tran Gia Khanh', 3.0, 'High', 'Done', '14/09/2026', '14/09/2026', 3.0, 'Completed on time'),
    ('T37.2', 'US37', 'Perform security audit and PBKDF2 hashing benchmark unit tests', 'Vu Tien Dat', 4.0, 'High', 'Done', '14/09/2026', '14/09/2026', 4.0, 'Completed on time'),
    ('T05.5', 'US05', 'Test RBAC security rules preventing unauthorized access to Admin screens', 'Vu Tien Dat', 3.0, 'High', 'Done', '14/09/2026', '14/09/2026', 3.0, 'Completed on time'),
    ('T15.4', 'US15', 'Test Category Tag layout responsiveness across diverse device screen sizes', 'Nguyen An Cuong', 2.0, 'Medium', 'Done', '14/09/2026', '14/09/2026', 2.0, 'Completed on time'),
    ('T01.0', 'US38', 'Sprint 01 Review, Retrospective and Sprint 02 Backlog refinement', 'Nguyen Duc Manh', 2.0, 'Medium', 'Done', '14/09/2026', '14/09/2026', 2.0, 'Completed on time'),
]
for idx, t in enumerate(s1_detailed_tasks_en):
    r = 17 + idx
    safe_set(ws_td, r, 2, t[0])
    safe_set(ws_td, r, 3, t[1])
    safe_set(ws_td, r, 4, t[2])
    safe_set(ws_td, r, 5, t[3])
    safe_set(ws_td, r, 6, t[4])
    safe_set(ws_td, r, 7, t[5])
    safe_set(ws_td, r, 8, t[6])
    safe_set(ws_td, r, 9, t[7])
    safe_set(ws_td, r, 10, t[8])
    safe_set(ws_td, r, 11, t[9])
    safe_set(ws_td, r, 12, t[10])

print("Theo dõi tiến độ (English) updated successfully.")

# ==========================================
# 4. SHEET: Features (English)
# ==========================================
ws_ft = wb['Features']
features_list_en = [
    ('F01', 'Authentication & Account Management (Auth, PBKDF2 & SecureStorage)'),
    ('F02', 'Profile Management (Profile UI & Firestore User Sync)'),
    ('F03', 'Event Discovery & Listing (Event List Screen - Pull-to-Refresh & Pagination)'),
    ('F04', 'Event Detail & Schedule (Event Detail, Schedule & Guidelines)'),
    ('F05', 'Event Registration & Lifecycle Management (Registration & Event CRUD)'),
    ('F06', 'My Registered Events & Notifications (Registered Events & 2-tier Modal)'),
    ('F07', 'Category Management (Event Categories & Pill Chips Filter)'),
    ('F08', 'QR Camera Scanner, Photo Check-in & ĐRL Ledger (ZXing QR, MediaPicker & Points)'),
    ('F09', 'Real-time Attendance Monitoring & CSV Export (Live Monitor & UTF-8 BOM CSV)'),
    ('F10', 'System Settings & Cloud Synchronization (Theme Switching & Cloud Sync)'),
]
for idx, (fid, fname) in enumerate(features_list_en):
    r = 3 + idx
    safe_set(ws_ft, r, 1, fid)
    safe_set(ws_ft, r, 2, fname)

print("Features (English) updated successfully.")

# ==========================================
# 5. SHEET: Rủi ro (Risks - English)
# ==========================================
ws_rr = wb['Rủi ro']
safe_set(ws_rr, 1, 1, 'PROJECT RISKS ASSESSMENT & MITIGATION MEASURES')
safe_set(ws_rr, 2, 1, 'Evaluation levels: Low / Medium / High / Critical')
safe_set(ws_rr, 4, 1, 'No.')
safe_set(ws_rr, 4, 2, 'Risk Description')
safe_set(ws_rr, 4, 3, 'Probability')
safe_set(ws_rr, 4, 4, 'Impact Level')
safe_set(ws_rr, 4, 5, 'Mitigation Strategy')
safe_set(ws_rr, 4, 6, 'Preventive Measure')

risks_en = [
    (1.0, 'Camera hardware permission denied during QR code attendance scanning', 'Medium', 'High',
     'Show an intuitive guidance popup directing the user to device Settings + Provide a manual check-in fallback button',
     'Proactively check and request Camera permission before launching scanner view'),
    (2.0, 'Unstable internet connectivity at crowded campus event hall', 'High', 'Medium',
     'Cache session and check-in transactions locally in SecureStorage, automatically synchronize when connection resumes',
     'Implement offline caching and network connectivity checks before initiating Cloud Firestore requests'),
    (3.0, 'Captured proof image file size too large, degrading app performance and bandwidth', 'Medium', 'Medium',
     'Automatically downsample and compress images (JPEG 80%) before sending payload to Cloud Firestore',
     'Enforce maximum resolution limits when capturing photos via MediaPicker'),
    (4.0, 'Concurrent check-in traffic surge at venue entrance causing bottlenecks', 'Medium', 'High',
     'Leverage Google Cloud Firestore serverless auto-scaling architecture with batch writes to prevent race conditions',
     'Optimize Firestore composite queries and implement optimistic local UI updates'),
    (5.0, 'CSV attendance report font distortion when opening on Microsoft Excel (Vietnamese characters)', 'High', 'Low',
     'Prepend UTF-8 BOM prefix (\\uFEFF) to the generated CSV file before saving and triggering native OS share',
     'Enforce UTF-8 BOM encoding across all data export utilities in the .NET MAUI codebase'),
    (6.0, 'Session token or user password leakage on client device', 'Low', 'Critical',
     'Hash passwords using PBKDF2-SHA256 with random cryptographic salts; store tokens exclusively in SecureStorage',
     'Never store plaintext passwords or output authorization tokens to debug console logs'),
]
for idx, (stt, name, prob, impact, sol, prev) in enumerate(risks_en):
    r = 5 + idx
    safe_set(ws_rr, r, 1, stt)
    safe_set(ws_rr, r, 2, name)
    safe_set(ws_rr, r, 3, prob)
    safe_set(ws_rr, r, 4, impact)
    safe_set(ws_rr, r, 5, sol)
    safe_set(ws_rr, r, 6, prev)

print("Rủi ro (English) updated successfully.")

# ==========================================
# 6. SHEET: Product backlog (All 38 USs in English)
# ==========================================
ws_pb = wb['Product backlog']
safe_set(ws_pb, 1, 1, 'Product Backlog')
safe_set(ws_pb, 2, 1, 'Project Objective')
safe_set(ws_pb, 2, 2, ('Develop a mobile application to manage campus activities, allowing users to:\n'
                       '- View upcoming campus activities and details\n'
                       '- Register for events and receive notifications\n'
                       '- Check in via hardware QR camera or photo proof\n'
                       '- Track personal extra-curricular participation and training points (ĐRL)\n'
                       '- Monitor attendees in real time and export UTF-8 CSV reports\n'))
safe_set(ws_pb, 3, 1, 'ID')
safe_set(ws_pb, 3, 2, 'User Story')
safe_set(ws_pb, 3, 3, 'Story Point')
safe_set(ws_pb, 3, 4, 'Explanation / Technical Notes')
safe_set(ws_pb, 3, 5, 'Acceptance Criteria (AC)')
safe_set(ws_pb, 3, 6, 'Feature Id')
safe_set(ws_pb, 3, 7, 'Priority Point')

# Example row
safe_set(ws_pb, 4, 1, 'Example')
safe_set(ws_pb, 4, 2, 'As a student, I want to receive notifications about newly published events matching my interests so that I do not miss out.')
ws_pb['C4'] = 2.0
safe_set(ws_pb, 4, 4, 'System delivers push notification and displays in-app alert banner on the home screen.')
safe_set(ws_pb, 4, 5, ('AC1: When a new event matching interested categories is published, a badge displays on the bell icon.\n'
                       'AC2: Tapping the notification navigates directly to the corresponding EventDetailPage.\n'
                       'AC3: Users can toggle notification preferences on the Settings screen.'))
safe_set(ws_pb, 4, 6, 'F06')
ws_pb['G4'] = 3

us_38_en = {
    'US01': ('As a student, I want to view detailed event information so that I can understand the schedule, venue, capacity, and prepare accordingly.', 2, 'F04', 1,
             'View complete event details on EventDetailPage connected to Cloud Firestore.',
             'AC1: Tapping an event card navigates smoothly to EventDetailPage via .NET MAUI Shell.\nAC2: Displays complete event metadata: Title, banner, start/end time, venue, speaker, capacity, and training points (ĐRL).\nAC3: Displays individual registration status (Unregistered, Registered, Checked-in).'),
    'US02': ('As a student, I want to filter the event list by organizer or hosting club so that I can easily discover activities run by specific student organizations.', 6, 'F08', 1,
             'Filter events by Organizer/Club on EventListPageModel querying Firestore.',
             'AC1: Search bar and filter selector allow choosing or searching by organizer name.\nAC2: The event list refreshes automatically to show only activities hosted by the selected entity.'),
    'US03': ('As a participant, I want to view the detailed timeline and guidelines of an event so that I can arrive on time and prepare proper attire.', 2, 'F04', 3,
             'Event timeline, agenda, and special guidelines block on EventDetailPage.',
             'AC1: Displays check-in time, opening remarks, main sessions, and conclusion timestamps.\nAC2: Displays alerts regarding dress code or required items for attendance.'),
    'US04': ('As a registered student, I want to receive reminder notifications before an event starts so that I do not miss the activity.', 8, 'F06', 3,
             'EventNotificationModal displaying upcoming registered events within 24 hours.',
             'AC1: Floating 2-tier modal lists upcoming registered events.\nAC2: Features a pinned Close button at the footer and instant navigation to event details.'),
    'US05': ('As an admin, I want to manage user accounts on the system so that I can control access privileges and ensure security.', 6, 'F01', 1,
             'UserManagementPage connected to UserService synchronized with Cloud Firestore.',
             'AC1: UserManagementPage displays paginated user accounts fetched from Cloud Firestore.\nAC2: Supports search by Name, Email, and Student ID.\nAC3: Filter by user roles (Admin, Manager, Student).\nAC4: Supports smooth pagination and infinite scrolling.'),
    'US06': ('As a student, I want to filter events by category (Seminar, Sports, Volunteer...) so that I can quickly discover activities I care about.', 3, 'F06', 2,
             'Horizontal pill chips category filter bar at the top of the event list.',
             'AC1: Horizontal category pill chips displayed at the top of the screen.\nAC2: Selecting a category immediately filters the event list.\nAC3: Selecting "All" displays all active events.'),
    'US07': ('As a user, I want to view and update my personal profile so that my information remains accurate and current.', 4, 'F02', 2,
             'ProfilePage displaying data from SecureStorage and Firestore, allowing profile edits.',
             'AC1: ProfilePage displays logged-in user profile from SecureStorage and Firestore.\nAC2: Allows editing Full Name, Phone Number, and uploading avatar image.\nAC3: Saving changes updates Firestore and displays a success alert.'),
    'US08': ('As a user, I want to switch between Light and Dark themes so that the app matches my visual preference.', 4, 'F10', 3,
             'Theming settings in SettingsPage with dynamic resource dictionary swapping.',
             'AC1: Settings screen provides Light/Dark theme toggle.\nAC2: The app UI immediately adjusts colors according to Microsoft Fluent Blue standards.\nAC3: Selected theme preference is persisted across app sessions.'),
    'US09': ('As an event organizer, I want to assign volunteer roles and tasks so that event operations run smoothly.', 7, 'F09', 4,
             'Volunteer task assignment and coordination panel on event management screens.',
             'AC1: Organizers can assign volunteers to check-in, logistics, or technical roles.\nAC2: Volunteers receive assigned task notifications in their dashboard.'),
    'US10': ('As an admin, I want an advanced event dashboard to filter activities by date range, venue, and status.', 6, 'F09', 3,
             'Multi-criteria event tracking dashboard on AdminDashboardPage.',
             'AC1: Filter events by status (Upcoming, Ongoing, Finished, Cancelled).\nAC2: Real-time event summary counters (Total Events, Active Attendees).'),
    'US11': ('As a user, I want to see training points (ĐRL) and remaining seats on each event card.', 2, 'F04', 2,
             'Badge chips displaying training points (ĐRL) and capacity on event cards.',
             'AC1: Event cards feature an eye-catching badge displaying training points.\nAC2: Displays remaining seat counter (e.g., 45/100 seats available).'),
    'US12': ('As a student, I want to register for an event or cancel registration with a single tap.', 2, 'F05', 2,
             'Register / Cancel Registration button on EventDetailPage.',
             'AC1: Tap "Register Now" to write registration record to Firestore.\nAC2: If already registered, shows "Cancel Registration" with confirmation dialog.'),
    'US13': ('As an event manager, I want to create, update, schedule, and manage events so that I can organize campus activities effectively.', 7, 'F05', 1,
             'ManageEventsPage providing full CRUD capabilities on Cloud Firestore.',
             'AC1: Event creation form with Title, Description, Date/Time, Venue, Capacity, Category.\nAC2: Edit existing event details and update Firestore in real time.\nAC3: Delete/Cancel event with confirmation dialog.'),
    'US14': ('As a student, I want to submit live questions to event speakers so that I can engage during Q&A sessions.', 7, 'F04', 4,
             'Interactive speaker Q&A submission module on EventDetailPage.',
             'AC1: Students can post questions during ongoing events.\nAC2: Questions appear in real-time moderator queue.'),
    'US15': ('As a student, I want to see category tags on event cards so that I can quickly identify event types.', 1, 'F06', 1,
             'Category Tag pill badges on event cards (Seminar, Sports, Volunteer, Academic).',
             'AC1: Category tag displayed prominently on each event card.\nAC2: Proper background color and label for each category.\nAC3: Tapping a tag filters the event list by that category.'),
    'US16': ('As a participant, I want to rate and submit feedback after an event so that organizers can improve future activities.', 7, 'F06', 4,
             'Post-event 5-star rating and review feedback dialog.',
             'AC1: Rate event from 1 to 5 stars.\nAC2: Submit written comments; feedback is stored in Firestore.'),
    'US17': ('As a participant, I want to scan a QR code using my phone camera so that I can check in quickly without waiting in line.', 8, 'F06', 2,
             'Hardware camera QR scanning via ZXing.Net.Maui.Controls on QrScannerPage.',
             'AC1: Camera scanner preview activates in QrScannerPage.\nAC2: Real-time QR barcode recognition with 2.5s debounce filter.\nAC3: Instant attendance recording to Cloud Firestore attendances collection.'),
    'US18': ('As a participant, I want to receive an instant confirmation notification after checking in so that I know my attendance is recorded.', 5, 'F06', 3,
             '2-tier EventNotificationModal triggered immediately upon successful check-in.',
             'AC1: Confirmation modal pops up upon check-in success.\nAC2: Displays event title, check-in timestamp, and awarded training points (ĐRL).\nAC3: Pinned Close button at footer dismisses modal cleanly.'),
    'US19': ('As a student, I want to check my attendance history and training points ledger so that I can track my extra-curricular progress.', 4, 'F06', 2,
             'AttendanceHistoryPage and Training Points (ĐRL) Summary ledger.',
             'AC1: Displays total accumulated training points (ĐRL).\nAC2: Chronological list of attended events with verification status.\nAC3: Filter history by semester or academic year.'),
    'US20': ('As an event manager, I want to see a real-time list of attendees so that I can monitor participation during the event.', 5, 'F09', 2,
             'EventAttendanceReportPage displaying live attendee list from Cloud Firestore.',
             'AC1: Real-time attendee counter showing total registered vs checked-in.\nAC2: Live list of attendees with student name, ID, time, and check-in method.\nAC3: Search attendee by name or student ID.'),
    'US21': ('As a participant, I want to check in via photo proof with a manual fallback option when QR scanning is unavailable.', 5, 'F09', 2,
             'PhotoAttendancePage integrating MediaPicker camera capture with manual fallback button.',
             'AC1: Capture photo from camera or pick from gallery.\nAC2: Preview image, add proof notes, and submit to Firestore.\nAC3: Manual fallback button allows text-based justification if camera fails.'),
    'US22': ('As an event manager, I want to export attendance reports to a UTF-8 CSV file with BOM so that I can open it in Excel without font distortion.', 6, 'F09', 3,
             'CSV Export Service with UTF-8 BOM encoding and native OS file sharing.',
             'AC1: Generates CSV report with: No, Student ID, Full Name, Class, Email, Check-in Time, Method, Points.\nAC2: Prepend UTF-8 BOM (\\uFEFF) to ensure perfect Vietnamese character rendering in Microsoft Excel.\nAC3: Triggers native OS share sheet (Zalo, Drive, Gmail).'),
    'US23': ('As an admin, I want to configure allowed attendance methods (QR Code, Photo Proof, Manual) per event.', 9, 'F09', 5,
             'Attendance method configuration switches on ManageEventsPage.',
             'AC1: Toggle check-in modes (QR / Photo / Manual) per event.\nAC2: Mobile app dynamically enables only the permitted check-in modes for that event.'),
    'US24': ('As an admin, I want to track arrival timestamps and automatically detect late check-ins based on event schedule.', 8, 'F09', 4,
             'Late check-in evaluation logic comparing check-in timestamp with event start time.',
             'AC1: Records exact check-in timestamp.\nAC2: Tags attendance as "On-time" or "Late" based on event rules.'),
    'US25': ('As an admin, I want to detect duplicate or anomalous check-ins so that attendance data remains completely reliable.', 9, 'F09', 4,
             'Duplicate check-in prevention and validation rules on Firestore transactions.',
             'AC1: Prevents double check-in for the same student on the same event.\nAC2: Rejects check-ins attempted before event registration period opens.'),
    'US26': ('As a new user, I want to register an account with my email and password so that I can access the system.', 5, 'F01', 1,
             'RegisterPage connected to AuthenticationService with PBKDF2-SHA256 password hashing.',
             'AC1: Input fields for Full Name, Email, Password, Confirm Password.\nAC2: Client-side validation for email format and password strength.\nAC3: Submits account data to Firestore and navigates to LoginPage.'),
    'US27': ('As a user, I want to request a password reset email if I forget my password so that I can regain account access.', 3, 'F01', 4,
             'Forgot password request interface and validation flow.',
             'AC1: Input field for registered email address.\nAC2: Validates email existence and triggers reset flow.'),
    'US28': ('As a registered user, I want to log in with my credentials so that I can access my personalized dashboard and activities.', 5, 'F01', 1,
             'LoginPage verifying PBKDF2 password hashes, saving session to SecureStorage, and navigating via Shell RBAC.',
             'AC1: Input fields for Email/ID and Password with visibility toggle.\nAC2: Verifies credentials against Cloud Firestore PBKDF2 hash.\nAC3: Stores session token in SecureStorage and navigates based on role (Admin vs Student).'),
    'US29': ('As a user, I want biometric login (Fingerprint / FaceID) so that I can log in quickly and securely.', 8, 'F01', 3,
             'Biometric authentication support via device biometric sensors.',
             'AC1: Prompts for biometric authentication when enabled.\nAC2: Auto-logs in upon successful fingerprint / FaceID match.'),
    'US30': ('As a user, I want to log out securely so that my session is cleared from the device.', 3, 'F01', 4,
             'Logout button on ProfilePage clearing SecureStorage and resetting Shell navigation.',
             'AC1: Tapping Logout shows confirmation prompt.\nAC2: Clears session data from SecureStorage and redirects to LoginPage.'),
    'US31': ('As a student, I want to view a list of all upcoming campus events with key details so that I can quickly browse activities.', 3, 'F03', 1,
             'MainPage event list with Pull-to-Refresh and CollectionView cards.',
             'AC1: Displays list of upcoming events with banner, title, date, venue.\nAC2: Pull-to-Refresh reloads data from Cloud Firestore.\nAC3: EmptyView displayed when no events match.'),
    'US32': ('As a user, I want a search bar on the event list so that I can find events by title or keywords.', 3, 'F03', 3,
             'Search bar with debounced filtering on MainPage and EventListPage.',
             'AC1: Typing keywords filters event list dynamically.\nAC2: Clear button resets search results immediately.'),
    'US33': ('As an admin, I want to create, edit, and delete event categories so that activities are classified consistently.', 5, 'F07', 2,
             'ManageCategoriesPage providing full CRUD for event categories on Cloud Firestore.',
             'AC1: Lists all categories with icon and color badges.\nAC2: Add new category dialog with name and icon selection.\nAC3: Delete/edit category with confirmation.'),
    'US34': ('As a student, I want to browse events grouped by category on the event list screen.', 3, 'F07', 2,
             'Category pill filtering and grouped event display.',
             'AC1: Events can be filtered by category pills.\nAC2: Category metadata synchronized with Cloud Firestore.'),
    'US35': ('Develop an account management service (create, update, lock/unlock accounts) on Cloud Firestore.', 3, 'F01', 1,
             'UserService handling Firestore collection users CRUD and IsLocked status updates.',
             'AC1: Creates account in Firestore users collection when data is valid.\nAC2: Validates required fields (Email, FullName, Role, PasswordHash).\nAC3: Updates account details successfully.\nAC4: Locks/unlocks account updating IsLocked field.\nAC5: Returns standard ApiResponse<T> responses.'),
    'US36': ('Develop an account registration service (email validation, PBKDF2 password encryption).', 3, 'F01', 1,
             'AuthenticationService registration method with PBKDF2-SHA256 salted hashing.',
             'AC1: Validates student email format.\nAC2: Checks duplicate email on Firestore.\nAC3: Hashes password with PBKDF2-SHA256 and cryptographic salt.\nAC4: Never stores plaintext passwords.\nAC5: Sets default role to Student.'),
    'US37': ('Build an authentication service (verify credentials, manage tokens in SecureStorage).', 3, 'F01', 1,
             'AuthenticationService login method verifying PBKDF2 hashes and managing SecureStorage.',
             'AC1: Queries user by email on Firestore.\nAC2: Verifies input password against stored PBKDF2 hash.\nAC3: Denies login if account is locked (IsLocked=true).\nAC4: Generates session and persists to SecureStorage.\nAC5: Unit tested for all success and error scenarios.'),
    'US38': ('Build an event service to query events along with their associated category information.', 2, 'F06', 1,
             'EventService querying Firestore events with Category metadata integration.',
             'AC1: Fetches events list successfully from Firestore.\nAC2: Each event contains associated CategoryId and CategoryName.\nAC3: Supports filtering by CategoryId.\nAC4: Returns empty list gracefully if no events match.'),
}

for i in range(1, 39):
    uid = f'US{i:02d}'
    r = 4 + i
    if uid in us_38_en:
        story, sp, fid, prio, explain, ac = us_38_en[uid]
        safe_set(ws_pb, r, 1, uid)
        safe_set(ws_pb, r, 2, story)
        ws_pb.cell(r, 3, float(sp))
        safe_set(ws_pb, r, 4, explain)
        safe_set(ws_pb, r, 5, ac)
        safe_set(ws_pb, r, 6, fid)
        ws_pb.cell(r, 7, float(prio))

print("Product backlog (All 38 USs in English) updated successfully.")

# ==========================================
# 7. SPRINT BACKLOG & TASKS: SPRINT 01
# ==========================================
ws_s1b = wb['Sprint 01 backlog']
safe_set(ws_s1b, 1, 1, 'ID')
safe_set(ws_s1b, 1, 2, 'User Story')
safe_set(ws_s1b, 1, 3, 'Story Point')
safe_set(ws_s1b, 1, 4, 'Explanation / Technical Notes')
safe_set(ws_s1b, 1, 5, 'Acceptance Criteria (AC)')
safe_set(ws_s1b, 1, 6, 'Feature Id')
safe_set(ws_s1b, 1, 7, 'Priority Point')
safe_set(ws_s1b, 1, 8, 'AC')

s1_uss = ['US26', 'US28', 'US15', 'US05', 'US35', 'US36', 'US37', 'US38']
for idx, uid in enumerate(s1_uss):
    r = 2 + idx
    if uid in us_38_en:
        story, sp, fid, prio, explain, ac = us_38_en[uid]
        safe_set(ws_s1b, r, 1, uid)
        safe_set(ws_s1b, r, 2, story)
        ws_s1b.cell(r, 3, float(sp))
        safe_set(ws_s1b, r, 4, explain)
        safe_set(ws_s1b, r, 5, ac)
        safe_set(ws_s1b, r, 6, fid)
        ws_s1b.cell(r, 7, float(prio))
        safe_set(ws_s1b, r, 8, ac)

# Tasks sprint 01
ws_t1 = wb['Tasks srpint 01']
safe_set(ws_t1, 1, 1, '')
safe_set(ws_t1, 1, 2, 'User Story')
safe_set(ws_t1, 1, 3, 'Task ID')
safe_set(ws_t1, 1, 4, 'Task Name')
safe_set(ws_t1, 1, 5, 'Estimate Time (hours)')
safe_set(ws_t1, 1, 6, 'Assignee')
safe_set(ws_t1, 1, 7, 'Acceptance Criteria (AC)')

tasks_s1_en = [
    # US05
    ('US05', 'Account Management', 'T05.1', 'Develop User Management UI (UserManagementPage - MAUI)', 3.0, 'Nguyen An Cuong',
     'AC 1.1: Display user cards with Avatar, Name, Email/ID, Role tag (Admin/Manager/Student), and Status (Active/Locked).\n'
     'AC 1.2: Search bar filters users by name, student ID, email with debounced typing.\n'
     'AC 1.3: Filter chips for roles and statuses.\n'
     'AC 1.4: Pull-to-Refresh and infinite scrolling support.'),
    ('US05', 'Account Management', 'T05.2', 'Implement Role-Based Access Control logic on Firestore', 4.0, 'Nguyen Van An',
     'AC 05.2.1: Admins can update user roles and persist changes to Cloud Firestore.\n'
     'AC 05.2.2: Verify role updates take effect immediately in app navigation.'),
    ('US05', 'Account Management', 'T05.3', 'Connect User Management UI with UserService (Cloud Firestore)', 3.0, 'Tran Gia Khanh',
     'AC 05.3.1: Fetch user list directly from Cloud Firestore REST API.\n'
     'AC 05.3.2: Graceful error handling for network disconnection or permission denial.'),
    ('US05', 'Account Management', 'T05.4', 'Test User Management functionality (create, edit, lock, RBAC)', 3.0, 'Vu Tien Dat',
     'AC 05.4.1: Test account creation with valid credentials and PBKDF2 hashing.\n'
     'AC 05.4.2: Test profile updates.\n'
     'AC 05.4.3: Test lock/unlock preventing locked users from logging in.\n'
     'AC 05.4.4: Verify RBAC access controls.'),
    ('Total US05', '', '', '', '13 hours', '', ''),

    # US35
    ('US35', 'Account Management Service', 'T35.1', 'Build UserService for CRUD account operations on Cloud Firestore', 10.0, 'Nguyen Duc Manh',
     'AC1: Service creates accounts in Firestore users collection.\n'
     'AC2: Validates mandatory fields (Email, FullName, Role, PasswordHash).\n'
     'AC3: Updates account details successfully.\n'
     'AC4: Rejects updates on non-existent accounts with proper error message.\n'
     'AC5: Returns structured ApiResponse<T> objects.\n'
     'AC6: Direct serverless Firestore integration without intermediate server.'),
    ('US35', 'Account Management Service', 'T35.2', 'Implement Lock/Unlock Account logic & login integration test', 8.0, 'Nguyen Van An',
     'AC 35.2.1: Lock account successfully updating IsLocked=true on Firestore.\n'
     'AC 35.2.2: Unlock account successfully updating IsLocked=false.\n'
     'AC 35.2.3: Locked accounts cannot authenticate through login service.\n'
     'AC 35.2.4: Integration test between UserService and AuthenticationService.'),
    ('Total US35', '', '', '', '18 hours', '', ''),

    # US26
    ('US26', 'User Registration', 'T26.1', 'Develop Registration UI (RegisterPage - MAUI)', 2.0, 'Nguyen An Cuong',
     'AC 1.1: Clear input fields: Full Name, Email, Password, Confirm Password.\n'
     'AC 1.2: Prominent "Register" action button with Blue Theme.\n'
     'AC 1.3: Navigation link "Already have an account? Log In" leading to LoginPage.\n'
     'AC 1.4: Terms of Service agreement checkbox.'),
    ('US26', 'User Registration', 'T26.3', 'Validate client-side registration inputs (email regex, password strength)', 2.0, 'Tran Gia Khanh',
     'AC 26.3.1: Validates standard student email regex format.\n'
     'AC 26.3.2: Validates password strength (minimum 6 characters, letters and numbers).\n'
     'AC 26.3.3: In-place validation error labels beneath input fields.'),
    ('US26', 'User Registration', 'T26.4', 'Connect Registration UI with AuthenticationService & error handling', 2.0, 'Tran Gia Khanh',
     'AC 26.4.1: Send registration payload to AuthenticationService.\n'
     'AC 26.4.2: Show loading indicator during network request.\n'
     'AC 26.4.3: Display success alert and automatically navigate to LoginPage.'),
    ('US26', 'User Registration', 'T26.5', 'Test registration workflows (Unit tests & Manual tests)', 2.0, 'Vu Tien Dat',
     'AC 26.5.1: Test successful registration flow.\n'
     'AC 26.5.2: Test duplicate email, password mismatch, and missing fields error cases.'),
    ('Total US26', '', '', '', '8 hours', '', ''),

    # US36
    ('US36', 'Registration Service', 'T36.1', 'Implement serverless account registration with PBKDF2-SHA256 password hashing', 10.0, 'Nguyen An Cuong',
     'AC 36.1.1: Register account successfully with valid credentials.\n'
     'AC 36.1.2: Validate email uniqueness on Cloud Firestore.\n'
     'AC 36.1.3: Hash password using PBKDF2-SHA256 with random cryptographic salt.\n'
     'AC 36.1.4: Never persist plaintext passwords.\n'
     'AC 36.1.5: Assign default Student role to newly registered accounts.'),
    ('Total US36', '', '', '', '10 hours', '', ''),

    # US28
    ('US28', 'User Login', 'T28.1', 'Develop Login UI (LoginPage - MAUI) with password visibility toggle', 2.0, 'Nguyen An Cuong',
     'AC 1.1: Input fields for Email/ID and Password with eye toggle icon.\n'
     'AC 1.2: Prominent "Log In" button styled with Blue Theme #2563EB.\n'
     'AC 1.3: Navigation link to RegisterPage.\n'
     'AC 1.4: Clear error message display on authentication failure.'),
    ('US28', 'User Login', 'T28.3', 'Store session tokens and user credentials securely in SecureStorage', 2.0, 'Tran Gia Khanh',
     'AC 28.3.1: Save auth token and user profile securely in device SecureStorage.\n'
     'AC 28.3.2: Restore login session automatically upon subsequent app launches.\n'
     'AC 28.3.3: Clear SecureStorage completely upon logout.'),
    ('US28', 'User Login', 'T28.4', 'Connect Login UI with AuthenticationService & Shell navigation by role', 1.0, 'Nguyen Duc Manh',
     'AC 28.4.1: Connect Login UI to AuthenticationService verification.\n'
     'AC 28.4.2: Intelligent Shell navigation by role (Admin -> Admin Dashboard, Student -> Home).\n'
     'AC 28.4.3: Disable login button and display loading spinner while processing.'),
    ('US28', 'User Login', 'T28.5', 'Test login scenarios (valid credentials, wrong password, locked account)', 2.0, 'Vu Tien Dat',
     'AC 28.5.1: Successful login with valid credentials.\n'
     'AC 28.5.2: Reject incorrect email or password with secure error notice.\n'
     'AC 28.5.3: Display "Your account has been locked" when IsLocked=true.'),
    ('Total US28', '', '', '', '7 hours', '', ''),

    # US37
    ('US37', 'Authentication Service', 'T37.1', 'Build AuthenticationService with PBKDF2 verification and token generation', 9.0, 'Tran Gia Khanh',
     'AC 37.1.1: Verify credentials against Firestore PBKDF2-SHA256 password hash.\n'
     'AC 37.1.2: Reject login if password hash mismatch or account locked.\n'
     'AC 37.1.3: Generate session token and persist to SecureStorage.\n'
     'AC 37.1.4: Wrap response in standardized ApiResponse<LoginResult>.\n'
     'AC 37.1.5: Full unit test coverage for success and error conditions.'),
    ('Total US37', '', '', '', '9 hours', '', ''),

    # US15
    ('US15', 'Category Tag Display', 'T15.1', 'Develop Category Tag UI on event cards (Seminar, Volunteer, Sports...)', 1.0, 'Nguyen An Cuong',
     'AC 1.1: Category tag displayed prominently at top corner of each event card.\n'
     'AC 1.2: Display correct label and color for categories (Seminar, Volunteer, Sports, Arts, Academic).\n'
     'AC 1.3: Rounded pill chip design conforming to Blue Theme styling.\n'
     'AC 1.4: Tapping tag filters events by that category.'),
    ('US15', 'Category Tag Display', 'T15.3', 'Integrate Category Tag display with home event list & testing', 1.0, 'Vu Tien Dat',
     'AC 15.3.1: Category tags display consistently and synchronize with Firestore.\n'
     'AC 15.3.2: Verify responsive layout on Android and Windows without text truncation.'),
    ('Total US15', '', '', '', '2 hours', '', ''),

    # US38
    ('US38', 'Event Query Service', 'T38.1', 'Build EventService query to fetch events by category from Cloud Firestore', 12.0, 'Vu Tien Dat',
     'AC 38.1.1: Query events collection from Firestore successfully.\n'
     'AC 38.1.2: Each returned event includes CategoryId and CategoryName.\n'
     'AC 38.1.3: Filter events by CategoryId or retrieve all events when unfiltered.\n'
     'AC 38.1.4: Gracefully handle empty result sets without application crashing.\n'
     'AC 38.1.5: Test query performance and real-time UI binding.'),
    ('Total US38', '', '', '', '12 hours', '', ''),
]

for idx, t in enumerate(tasks_s1_en):
    r = 2 + idx
    safe_set(ws_t1, r, 1, t[0])
    safe_set(ws_t1, r, 2, t[1])
    safe_set(ws_t1, r, 3, t[2])
    safe_set(ws_t1, r, 4, t[3])
    ws_t1.cell(r, 5, t[4])
    safe_set(ws_t1, r, 6, t[5])
    safe_set(ws_t1, r, 7, t[6])

safe_set(ws_t1, 2 + len(tasks_s1_en) + 1, 2, 'Total Sprint 01 Execution Time')
safe_set(ws_t1, 2 + len(tasks_s1_en) + 1, 5, '79 hours')

print("Sprint 01 Backlog & Tasks (English) updated successfully.")

# ==========================================
# 8. SPRINT BACKLOG & TASKS: SPRINT 02
# ==========================================
ws_s2b = wb['Sprint 02 backlog']
safe_set(ws_s2b, 1, 1, 'ID')
safe_set(ws_s2b, 1, 2, 'User Story')
safe_set(ws_s2b, 1, 3, 'Story Point')
safe_set(ws_s2b, 1, 4, 'Explanation / Technical Notes')
safe_set(ws_s2b, 1, 5, 'Acceptance Criteria (AC)')
safe_set(ws_s2b, 1, 6, 'Feature Id')
safe_set(ws_s2b, 1, 7, 'Priority Point')
safe_set(ws_s2b, 1, 8, 'AC')

s2_uss = ['US31', 'US01', 'US13', 'US33', 'US07', 'US06', 'US32', 'US34']
for idx, uid in enumerate(s2_uss):
    r = 2 + idx
    if uid in us_38_en:
        story, sp, fid, prio, explain, ac = us_38_en[uid]
        safe_set(ws_s2b, r, 1, uid)
        safe_set(ws_s2b, r, 2, story)
        ws_s2b.cell(r, 3, float(sp))
        safe_set(ws_s2b, r, 4, explain)
        safe_set(ws_s2b, r, 5, ac)
        safe_set(ws_s2b, r, 6, fid)
        ws_s2b.cell(r, 7, float(prio))
        safe_set(ws_s2b, r, 8, ac)

# Tasks sprint 02
if 'Tasks sprint 02' not in wb.sheetnames:
    ws_t2 = wb.create_sheet('Tasks sprint 02')
else:
    ws_t2 = wb['Tasks sprint 02']

safe_set(ws_t2, 1, 1, '')
safe_set(ws_t2, 1, 2, 'User Story')
safe_set(ws_t2, 1, 3, 'Task ID')
safe_set(ws_t2, 1, 4, 'Task Name')
safe_set(ws_t2, 1, 5, 'Estimate Time (hours)')
safe_set(ws_t2, 1, 6, 'Assignee')
safe_set(ws_t2, 1, 7, 'Acceptance Criteria (AC)')

tasks_s2_en = [
    # US31
    ('US31', 'Event List Display', 'T31.1', 'Develop Event List UI on MainPage with Pull-to-Refresh', 4.0, 'Nguyen An Cuong',
     'AC 31.1.1: CollectionView displaying event cards with rounded corners, banner, time, and venue.\n'
     'AC 31.1.2: Pull-to-Refresh gesture synchronizes data directly from Cloud Firestore.'),
    ('US31', 'Event List Display', 'T31.2', 'Connect EventService to fetch event collection from Cloud Firestore', 2.0, 'Tran Gia Khanh',
     'AC 31.2.1: Query Firestore events collection and map to EventModel.\n'
     'AC 31.2.2: Handle connection loss and display EmptyView when no events exist.'),
    ('US31', 'Event List Display', 'T31.3', 'Test event list display and scrolling performance', 2.0, 'Vu Tien Dat',
     'AC 31.3.1: Verify smooth 60fps scrolling on Android and Windows.\n'
     'AC 31.3.2: Verify event data accuracy and image loading efficiency.'),
    ('Total US31', '', '', '', '8 hours', '', ''),

    # US01
    ('US01', 'Event Details View', 'T01.1', 'Develop Event Details UI (EventDetailPage - MAUI)', 3.0, 'Nguyen An Cuong',
     'AC 01.1.1: Complete event view with banner, date/time, venue, speaker, and points.\n'
     'AC 01.1.2: Sticky Register / Checked-in action button at page footer.'),
    ('US01', 'Event Details View', 'T01.2', 'Connect EventService to retrieve event details by ID from Firestore', 2.0, 'Tran Gia Khanh',
     'AC 01.2.1: Fetch event document by ID from Firestore.\n'
     'AC 01.2.2: Display current user registration status.'),
    ('US01', 'Event Details View', 'T01.3', 'Test event details navigation and data binding', 2.0, 'Vu Tien Dat',
     'AC 01.3.1: Verify navigation from home screen and event list.\n'
     'AC 01.3.2: Verify complete and accurate data rendering.'),
    ('Total US01', '', '', '', '7 hours', '', ''),

    # US13
    ('US13', 'Event Management', 'T13.1', 'Build EventService for CRUD event operations with Cloud Firestore', 8.0, 'Nguyen Duc Manh',
     'AC 13.1.1: Add new event document to Firestore events collection.\n'
     'AC 13.1.2: Update existing event details and schedule.\n'
     'AC 13.1.3: Delete / Cancel event safely with transaction consistency.'),
    ('US13', 'Event Management', 'T13.2', 'Develop event creation and editing UI (ManageEventsPage)', 6.0, 'Nguyen Van An',
     'AC 13.2.1: Comprehensive form: Title, Description, Time, Venue, Capacity, Points, Category.\n'
     'AC 13.2.2: Comprehensive client-side form validation.'),
    ('US13', 'Event Management', 'T13.3', 'Integrate event management logic and real-time state synchronization', 4.0, 'Tran Gia Khanh',
     'AC 13.3.1: Load managed events list for organizers.\n'
     'AC 13.3.2: Live list update immediately after add/edit/delete.'),
    ('US13', 'Event Management', 'T13.4', 'Test event management end-to-end workflows and permissions', 4.0, 'Vu Tien Dat',
     'AC 13.4.1: Test CRUD operations on event documents.\n'
     'AC 13.4.2: Verify only Admin and Manager roles can access event management.'),
    ('Total US13', '', '', '', '22 hours', '', ''),

    # US33
    ('US33', 'Category Management', 'T33.1', 'Build CategoryService for CRUD category operations on Cloud Firestore', 5.0, 'Nguyen Duc Manh',
     'AC 33.1.1: Fetch categories from Firestore categories collection.\n'
     'AC 33.1.2: Add, update, and delete event categories.'),
    ('US33', 'Category Management', 'T33.2', 'Develop Category Management UI (ManageCategoriesPage)', 5.0, 'Nguyen Van An',
     'AC 33.2.1: Card-based category list view.\n'
     'AC 33.2.2: Dialog form to create/edit category name, icon, and hex color code.'),
    ('US33', 'Category Management', 'T33.3', 'Test category management functionality', 2.0, 'Vu Tien Dat',
     'AC 33.3.1: Category CRUD operations work smoothly.\n'
     'AC 33.3.2: Newly created categories appear immediately in home screen filter.'),
    ('Total US33', '', '', '', '12 hours', '', ''),

    # US07
    ('US07', 'Profile Management', 'T07.1', 'Build UserService method to update user profile in Cloud Firestore', 4.0, 'Nguyen Duc Manh',
     'AC 07.1.1: Update profile data (Full Name, Phone, Avatar) on Firestore.\n'
     'AC 07.1.2: Update cached profile in SecureStorage.'),
    ('US07', 'Profile Management', 'T07.2', 'Develop User Profile UI (ProfilePage - .NET MAUI)', 4.0, 'Nguyen An Cuong',
     'AC 07.2.1: Display avatar, name, email, student ID, and role badge.\n'
     'AC 07.2.2: Profile editable fields and "Save Changes" button.'),
    ('US07', 'Profile Management', 'T07.3', 'Integrate and test profile update workflow', 3.0, 'Tran Gia Khanh',
     'AC 07.3.1: Profile updates successfully and displays confirmation.\n'
     'AC 07.3.2: Re-logging in preserves updated profile information.'),
    ('Total US07', '', '', '', '11 hours', '', ''),
]

for idx, t in enumerate(tasks_s2_en):
    r = 2 + idx
    safe_set(ws_t2, r, 1, t[0])
    safe_set(ws_t2, r, 2, t[1])
    safe_set(ws_t2, r, 3, t[2])
    safe_set(ws_t2, r, 4, t[3])
    ws_t2.cell(r, 5, t[4])
    safe_set(ws_t2, r, 6, t[5])
    safe_set(ws_t2, r, 7, t[6])

safe_set(ws_t2, 2 + len(tasks_s2_en) + 1, 2, 'Total Sprint 02 Execution Time')
safe_set(ws_t2, 2 + len(tasks_s2_en) + 1, 5, '60 hours')

print("Sprint 02 Backlog & Tasks (English) updated successfully.")

# ==========================================
# 9. SPRINT BACKLOG & TASKS: SPRINT 03
# ==========================================
if 'Sprint 03 backlog' not in wb.sheetnames:
    ws_s3b = wb.create_sheet('Sprint 03 backlog')
else:
    ws_s3b = wb['Sprint 03 backlog']

safe_set(ws_s3b, 1, 1, 'ID')
safe_set(ws_s3b, 1, 2, 'User Story')
safe_set(ws_s3b, 1, 3, 'Story Point')
safe_set(ws_s3b, 1, 4, 'Explanation / Technical Notes')
safe_set(ws_s3b, 1, 5, 'Acceptance Criteria (AC)')
safe_set(ws_s3b, 1, 6, 'Feature Id')
safe_set(ws_s3b, 1, 7, 'Priority Point')
safe_set(ws_s3b, 1, 8, 'AC')

s3_uss = ['US17', 'US18', 'US21', 'US23', 'US24']
for idx, uid in enumerate(s3_uss):
    r = 2 + idx
    if uid in us_38_en:
        story, sp, fid, prio, explain, ac = us_38_en[uid]
        safe_set(ws_s3b, r, 1, uid)
        safe_set(ws_s3b, r, 2, story)
        ws_s3b.cell(r, 3, float(sp))
        safe_set(ws_s3b, r, 4, explain)
        safe_set(ws_s3b, r, 5, ac)
        safe_set(ws_s3b, r, 6, fid)
        ws_s3b.cell(r, 7, float(prio))
        safe_set(ws_s3b, r, 8, ac)

# Tasks sprint 03
if 'Tasks sprint 03' not in wb.sheetnames:
    ws_t3 = wb.create_sheet('Tasks sprint 03')
else:
    ws_t3 = wb['Tasks sprint 03']

safe_set(ws_t3, 1, 1, '')
safe_set(ws_t3, 1, 2, 'User Story')
safe_set(ws_t3, 1, 3, 'Task ID')
safe_set(ws_t3, 1, 4, 'Task Name')
safe_set(ws_t3, 1, 5, 'Estimate Time (hours)')
safe_set(ws_t3, 1, 6, 'Assignee')
safe_set(ws_t3, 1, 7, 'Acceptance Criteria (AC)')

tasks_s3_en = [
    # US17
    ('US17', 'QR Code Attendance', 'T17.1', 'Integrate ZXing.Net.Maui.Controls camera scanner view into QrScannerPage', 8.0, 'Nguyen Duc Manh',
     'AC 17.1.1: Active camera preview with real-time barcode scanning engine.\n'
     'AC 17.1.2: Debounce filter of 2.5s prevents duplicate scans in rapid succession.'),
    ('US17', 'QR Code Attendance', 'T17.2', 'Implement QR ticket generator view and display modal', 4.0, 'Nguyen An Cuong',
     'AC 17.2.1: Render crisp QR ticket containing standardized payload.\n'
     'AC 17.2.2: Display student identity, event title, and ticket timestamp.'),
    ('US17', 'QR Code Attendance', 'T17.3', 'Connect QR scan event to Cloud Firestore attendances collection', 3.0, 'Tran Gia Khanh',
     'AC 17.3.1: Save check-in record with timestamp and student ID to Firestore.\n'
     'AC 17.3.2: Duplicate check-in prevention logic enforces single attendance.'),
    ('US17', 'QR Code Attendance', 'T17.4', 'Test QR scanning performance under diverse lighting and camera angles', 3.0, 'Vu Tien Dat',
     'AC 17.4.1: Barcode recognition response speed < 1 second.\n'
     'AC 17.4.2: Graceful handling of camera permission denial.'),
    ('Total US17', '', '', '', '18 hours', '', ''),

    # US21
    ('US21', 'Photo Proof Attendance', 'T21.1', 'Develop PhotoAttendancePage with MediaPicker camera capture and preview', 5.0, 'Nguyen An Cuong',
     'AC 21.1.1: Capture photo via MediaPicker or choose proof image from gallery.\n'
     'AC 21.1.2: Image preview display with caption/notes input field.'),
    ('US21', 'Photo Proof Attendance', 'T21.2', 'Implement image compression (JPEG 80%) & upload proof to Firestore', 4.0, 'Nguyen Duc Manh',
     'AC 21.2.1: Resize and compress image to avoid excessive payload overhead.\n'
     'AC 21.2.2: Store attendance proof document with Status=Pending/Approved.'),
    ('US21', 'Photo Proof Attendance', 'T21.3', 'Build Manual Attendance Fallback toggle and justification input', 4.0, 'Nguyen Van An',
     'AC 21.3.1: Provide toggle button for manual check-in when camera is unavailable.\n'
     'AC 21.3.2: Require student ID entry and justification notes.'),
    ('US21', 'Photo Proof Attendance', 'T21.4', 'Test photo capture, memory management, and permission flows', 3.0, 'Vu Tien Dat',
     'AC 21.4.1: Verify image capture works reliably on Android and Windows.\n'
     'AC 21.4.2: Verify proper memory disposal of Bitmap and Stream resources.'),
    ('Total US21', '', '', '', '16 hours', '', ''),

    # US18
    ('US18', 'Check-in Confirmation Modal', 'T18.1', 'Develop EventNotificationModal with fixed close button and scrollable body', 4.0, 'Nguyen An Cuong',
     'AC 18.1.1: 2-tier floating modal design with 250px scrollable content area.\n'
     'AC 18.1.2: Success badge, event title, and timestamp banner.'),
    ('US18', 'Check-in Confirmation Modal', 'T18.2', 'Wire check-in events to trigger confirmation modal with sound/vibration', 3.0, 'Tran Gia Khanh',
     'AC 18.2.1: Trigger modal immediately upon successful Firestore write.\n'
     'AC 18.2.2: Display awarded training points (ĐRL) prominently.'),
    ('US18', 'Check-in Confirmation Modal', 'T18.3', 'Test modal layout across mobile portrait and tablet landscape orientations', 2.0, 'Vu Tien Dat',
     'AC 18.3.1: Verify modal does not clip on small screens.\n'
     'AC 18.3.2: Backdrop dismiss behavior works correctly.'),
    ('Total US18', '', '', '', '9 hours', '', ''),

    # US23 & US24
    ('US23', 'Attendance Configuration', 'T23.1', 'Admin UI to configure check-in methods (QR / Photo / Manual) in ManageEventsPage', 4.0, 'Nguyen Van An',
     'AC 23.1.1: Checkbox group to enable/disable check-in options per event.\n'
     'AC 23.1.2: Mobile app dynamically enables permitted check-in modes.'),
    ('US24', 'Late Arrival Tracking', 'T24.1', 'Implement late arrival detection logic comparing timestamp with event schedule', 3.0, 'Nguyen Duc Manh',
     'AC 24.1.1: Compare check-in time against event start time.\n'
     'AC 24.1.2: Tag attendance document as "On-time" or "Late".'),
    ('US24', 'Late Arrival Tracking', 'T24.2', 'Test attendance rule enforcement and boundary timestamp conditions', 3.0, 'Vu Tien Dat',
     'AC 24.2.1: Test boundary conditions: check-in on time, 15m after, and after event.'),
    ('Total US23/US24', '', '', '', '10 hours', '', ''),
]

for idx, t in enumerate(tasks_s3_en):
    r = 2 + idx
    safe_set(ws_t3, r, 1, t[0])
    safe_set(ws_t3, r, 2, t[1])
    safe_set(ws_t3, r, 3, t[2])
    safe_set(ws_t3, r, 4, t[3])
    ws_t3.cell(r, 5, t[4])
    safe_set(ws_t3, r, 6, t[5])
    safe_set(ws_t3, r, 7, t[6])

safe_set(ws_t3, 2 + len(tasks_s3_en) + 1, 2, 'Total Sprint 03 Execution Time')
safe_set(ws_t3, 2 + len(tasks_s3_en) + 1, 5, '50 hours')

print("Sprint 03 Backlog & Tasks (English) updated successfully.")

# ==========================================
# 10. SPRINT BACKLOG & TASKS: SPRINT 04
# ==========================================
if 'Sprint 04 backlog' not in wb.sheetnames:
    ws_s4b = wb.create_sheet('Sprint 04 backlog')
else:
    ws_s4b = wb['Sprint 04 backlog']

safe_set(ws_s4b, 1, 1, 'ID')
safe_set(ws_s4b, 1, 2, 'User Story')
safe_set(ws_s4b, 1, 3, 'Story Point')
safe_set(ws_s4b, 1, 4, 'Explanation / Technical Notes')
safe_set(ws_s4b, 1, 5, 'Acceptance Criteria (AC)')
safe_set(ws_s4b, 1, 6, 'Feature Id')
safe_set(ws_s4b, 1, 7, 'Priority Point')
safe_set(ws_s4b, 1, 8, 'AC')

s4_uss = ['US04', 'US19', 'US20', 'US22', 'US25', 'US10']
for idx, uid in enumerate(s4_uss):
    r = 2 + idx
    if uid in us_38_en:
        story, sp, fid, prio, explain, ac = us_38_en[uid]
        safe_set(ws_s4b, r, 1, uid)
        safe_set(ws_s4b, r, 2, story)
        ws_s4b.cell(r, 3, float(sp))
        safe_set(ws_s4b, r, 4, explain)
        safe_set(ws_s4b, r, 5, ac)
        safe_set(ws_s4b, r, 6, fid)
        ws_s4b.cell(r, 7, float(prio))
        safe_set(ws_s4b, r, 8, ac)

# Tasks sprint 04
if 'Tasks sprint 04' not in wb.sheetnames:
    ws_t4 = wb.create_sheet('Tasks sprint 04')
else:
    ws_t4 = wb['Tasks sprint 04']

safe_set(ws_t4, 1, 1, '')
safe_set(ws_t4, 1, 2, 'User Story')
safe_set(ws_t4, 1, 3, 'Task ID')
safe_set(ws_t4, 1, 4, 'Task Name')
safe_set(ws_t4, 1, 5, 'Estimate Time (hours)')
safe_set(ws_t4, 1, 6, 'Assignee')
safe_set(ws_t4, 1, 7, 'Acceptance Criteria (AC)')

tasks_s4_en = [
    # US19
    ('US19', 'Training Points Ledger', 'T19.1', 'Develop AttendanceHistoryPage and Training Points Summary card', 5.0, 'Nguyen An Cuong',
     'AC 19.1.1: Display total accumulated training points (ĐRL) on summary card.\n'
     'AC 19.1.2: Group events by status (Attended, Registered, Absent).'),
    ('US19', 'Training Points Ledger', 'T19.2', 'Query student attendance records & calculate total ĐRL from Cloud Firestore', 4.0, 'Nguyen Duc Manh',
     'AC 19.2.1: Aggregated query for user attendances.\n'
     'AC 19.2.2: Compute training points sum accurately without client-side skew.'),
    ('US19', 'Training Points Ledger', 'T19.3', 'Test training points calculation and history list pagination', 2.0, 'Vu Tien Dat',
     'AC 19.3.1: Verify points tally against event definitions.\n'
     'AC 19.3.2: Verify zero points for absent events.'),
    ('Total US19', '', '', '', '11 hours', '', ''),

    # US20
    ('US20', 'Live Attendee Monitoring', 'T20.1', 'Develop EventAttendanceReportPage with attendee list & search bar', 4.0, 'Nguyen An Cuong',
     'AC 20.1.1: Display list of checked-in students with avatar, name, student ID, timestamp, method (QR/Photo).\n'
     'AC 20.1.2: Search attendees by student name or student ID.'),
    ('US20', 'Live Attendee Monitoring', 'T20.2', 'Implement real-time attendee counter and Firestore snapshot listener', 4.0, 'Nguyen Duc Manh',
     'AC 20.2.1: Live count of registered vs checked-in participants.\n'
     'AC 20.2.2: Instant update upon new check-in without manual refresh.'),
    ('US20', 'Live Attendee Monitoring', 'T20.3', 'Attendee filtering by class, faculty, and check-in status', 3.0, 'Nguyen Van An',
     'AC 20.3.1: Filter by checked-in status and search by student ID or name.\n'
     'AC 20.3.2: Quick statistics summary chips.'),
    ('US20', 'Live Attendee Monitoring', 'T20.4', 'Test real-time synchronization under multi-user concurrent check-in', 3.0, 'Vu Tien Dat',
     'AC 20.4.1: Simulate multiple concurrent check-ins.\n'
     'AC 20.4.2: Verify no duplicates or UI freezing.'),
    ('Total US20', '', '', '', '14 hours', '', ''),

    # US22
    ('US22', 'CSV Attendance Export', 'T22.1', 'Implement CSV export service with UTF-8 BOM encoding (\\uFEFF)', 4.0, 'Nguyen Duc Manh',
     'AC 22.1.1: Generate CSV containing: No, Student ID, Full Name, Class, Email, Check-in Time, Method, Points.\n'
     'AC 22.1.2: Prepend UTF-8 BOM prefix so Excel opens Vietnamese characters cleanly.'),
    ('US22', 'CSV Attendance Export', 'T22.2', 'Integrate native OS file sharing (Share.Default.RequestAsync)', 3.0, 'Tran Gia Khanh',
     'AC 22.2.1: Launch native OS share sheet (Zalo, Drive, Gmail, Save to Files).\n'
     'AC 22.2.2: Temporary file caching and proper clean-up.'),
    ('US22', 'CSV Attendance Export', 'T22.3', 'Test CSV file formatting across Excel Windows, Mac, and Google Sheets', 2.0, 'Vu Tien Dat',
     'AC 22.3.1: Open CSV in MS Excel, verify Vietnamese diacritics display properly.\n'
     'AC 22.3.2: Verify CSV column alignments and data integrity.'),
    ('Total US22', '', '', '', '9 hours', '', ''),

    # US04
    ('US04', 'Event Notifications', 'T04.1', 'Develop notification bell UI and badge counter on MainPage', 3.0, 'Nguyen An Cuong',
     'AC 04.1.1: Display bell icon with unread badge count.\n'
     'AC 04.1.2: Click bell to open upcoming events list.'),
    ('US04', 'Event Notifications', 'T04.2', 'Query upcoming registered events (< 24h) for current user from Firestore', 3.0, 'Tran Gia Khanh',
     'AC 04.2.1: Query registered events where StartTime is within 24 hours.\n'
     'AC 04.2.2: Display time countdown badges.'),
    ('Total US04', '', '', '', '6 hours', '', ''),

    # Polish & Final Packaging
    ('US25', 'DoD & Release Package', 'T25.1', 'Performance profiling, layout optimization & memory leak inspection', 3.0, 'Tran Gia Khanh',
     'AC 25.1.1: Ensure smooth 60fps scrolling on CollectionView.\n'
     'AC 25.1.2: Proper disposal of camera resources.'),
    ('US25', 'DoD & Release Package', 'T25.2', 'Automated end-to-end regression testing & Definition of Done audit', 3.0, 'Vu Tien Dat',
     'AC 25.2.1: All unit & integration tests pass 100%.\n'
     'AC 25.2.2: Verify DoD1 - DoD6 compliance.'),
    ('US25', 'DoD & Release Package', 'T25.3', 'Android APK release build packaging & installation verification', 2.0, 'Nguyen Duc Manh',
     'AC 25.3.1: Build signed release APK package.\n'
     'AC 25.3.2: Verify installation and launch on physical Android device.'),
    ('Total Packaging', '', '', '', '8 hours', '', ''),
]

for idx, t in enumerate(tasks_s4_en):
    r = 2 + idx
    safe_set(ws_t4, r, 1, t[0])
    safe_set(ws_t4, r, 2, t[1])
    safe_set(ws_t4, r, 3, t[2])
    safe_set(ws_t4, r, 4, t[3])
    ws_t4.cell(r, 5, t[4])
    safe_set(ws_t4, r, 6, t[5])
    safe_set(ws_t4, r, 7, t[6])

safe_set(ws_t4, 2 + len(tasks_s4_en) + 1, 2, 'Total Sprint 04 Execution Time')
safe_set(ws_t4, 2 + len(tasks_s4_en) + 1, 5, '45 hours')

print("Sprint 04 Backlog & Tasks (English) updated successfully.")

# Save to destination paths
dest_repo = 'Project Plan Campus Activity Manager (MAUI).xlsx'
dest_dl1 = r'C:\Users\Admin\Downloads\Project Plan Campus Activity Manager (MAUI).xlsx'
dest_dl2 = r'C:\Users\Admin\Downloads\Project_Plan_Campus_Activity_Manager__MAUI__.xlsx'

wb.save(dest_repo)
print(f"Saved to {dest_repo}")

try:
    wb.save(dest_dl1)
    print(f"Saved to {dest_dl1}")
except Exception as e:
    print(f"Error saving to {dest_dl1}: {e}")

try:
    wb.save(dest_dl2)
    print(f"Saved to {dest_dl2}")
except Exception as e:
    print(f"Error saving to {dest_dl2}: {e}")

print("ALL 14 SHEETS (4 SPRINTS IN ENGLISH) BUILT AND SAVED SUCCESSFULLY!")
