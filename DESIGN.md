# Zone Control 3 — Design Document

> **Game:** 7 Days to Die  
> **Mod version:** 3.0.0  
> **Assembly:** `ZoneControl3`

## 1. Overview

<!-- Describe the high-level purpose of the mod in one or two sentences. -->
> _What problem does this mod solve, or what experience does it add?_

## 2. Goals

<!-- List the primary goals this mod aims to achieve. -->

- [ ] Goal 1
- [ ] Goal 2
- [ ] Goal 3

## 3. Non-Goals

<!-- Explicitly state what this mod will NOT do, to keep scope clear. -->

- Non-goal 1
- Non-goal 2

## 4. Core Features

<!-- Break down each major feature. Add or remove sections as needed. -->

### 4.1 Feature Name

**Description:**  
<!-- What does this feature do? -->

**Affected game systems:**  
<!-- e.g. Loot, Zombies, World Generation, UI, etc. -->

**Implementation notes:**  
<!-- Any technical details, Harmony patches, or XML changes required. -->

---

## 5. Console and Config

### ⌨️Console Commands (when you press F1)

```text
+----------------+--------------------------------------------------------------+
| Command        | Description                                                  |
+----------------+--------------------------------------------------------------+
| modhelp        | List available commands with their descriptions              |
+----------------+--------------------------------------------------------------+
+----------------+--------------------------------------------------------------+
+----------------+--------------------------------------------------------------+
+----------------+--------------------------------------------------------------+
```
### ❄️Config file
The mod can also be configured by editing `modconfig.json`

```text
+---------------------+---------+--------------------------------------------------------------+
| Setting             | Default | Description                                                  |
+---------------------+---------+--------------------------------------------------------------+
+---------------------+---------+--------------------------------------------------------------+
+---------------------+---------+--------------------------------------------------------------+
| isDebug             | false   | Logs additional information that might be useful for         |
|                     |         | troubleshooting problems. You can generally leave this       |
|                     |         | turned off.                                                  |
+---------------------+---------+--------------------------------------------------------------+
```

## 6. Architecture
