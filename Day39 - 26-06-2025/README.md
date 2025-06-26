# Linux File Permissions & User Management Notes

Linux permission and user management concepts with Questions.

1. You have a file with permissions -rw-r--r– (644), and you run chmod +x file.sh. What happens?
2. What is the difference between chmod 744 file.txt and chmod u=rwx,go=r file.txt?
3. What is the sticky bit, and when should you use it?
4. You are told to give the owner full access, group only execute, and others no permissions. What symbolic command achieves this?
5. What is umask, and why is it important?
6. If the umask is 022, what are the default permissions for a new file and a new directory?
7. Why is umask often set to 002 in development environments but 027 or 077 in production?
8. useradd vs adduser

## 1. `chmod +x file.sh` on a File with `644` Permissions

### Original Permissions:

```bash
-rw-r--r--  (644)
```

### After Running:

```bash
chmod +x file.sh
```

### New Permissions:

```bash
-rwxr-xr--  (755)
```

### Effect:

- Adds execute (`x`) permission for user, group, and others.
- File becomes executable.
- Common for scripts (`.sh` files).


## 2. `chmod 744 file.txt` vs `chmod u=rwx,go=r file.txt`

Both commands set the same permissions:

```bash
-rwxr--r--  (744)
```

### Difference:

- `744` is numeric (octal) notation.
- `u=rwx,go=r` is symbolic notation.

Both grant:

- Full access to the **owner**
- Read-only to **group** and **others**


## 3. Sticky Bit

### What is it?

A special permission that:

- Is used on directories
- Prevents users from deleting or renaming files **they don't own**

### Common Use:

```bash
/tmp
```

### Example:

```bash
chmod +t /shared_folder
```

### Effect:

Only file owners (or root) can delete their files inside the directory.

## 4. Give Owner Full Access, Group Execute Only, Others None

### Command:

```bash
chmod u=rwx,g=x,o= file.txt
```

### Resulting Permissions:

```bash
-rwx--x---
```


## 5. What is `umask` and Why is It Important?

`umask` (User Mask) determines default permission bits _not_ set on new files/directories.

### System Defaults:

- **Files**: `666` (rw-rw-rw-)
- **Directories**: `777` (rwxrwxrwx)

### Example:

```bash
umask 022
```

### Resulting Permissions:

- Files: `644` -> `rw-r--r--`
- Dirs: `755` -> `rwxr-xr-x`

### Why It's Important:

- Enforces **default security policies**
- Prevents accidentally creating **overly accessible files**


## 6. If `umask = 022`, What Are Default Permissions?

| Type      | Base Permission | Umask | Result              |
| --------- | --------------- | ----- | ------------------- |
| File      | `666`           | `022` | `644` -> `rw-r--r--` |
| Directory | `777`           | `022` | `755` -> `rwxr-xr-x` |

## 7. Why Use Different `umask` Values?

### `002` – Development

- Result: Group can read/write
- Use Case: Shared team environments
- Example: `rw-rw-r--`, `rwxrwxr-x`

### `027` – Production

- Result: Others get no access, group has limited access
- Use Case: Controlled access
- Example: `rw-r-----`, `rwxr-x---`

### `077` – Secure Environments

- Result: Only user has access
- Use Case: Private/confidential files
- Example: `rw-------`, `rwx------`

## 8. `useradd` vs `adduser`

| Feature       | `useradd`             | `adduser`                            |
| ------------- | --------------------- | ------------------------------------ |
| Type          | Low-level binary      | High-level wrapper script            |
| Interactive   | No                    | Yes                                  |
| User-friendly | Less                  | More userfriendly                    |
| Used in       | All Linux distros     | Debian/Ubuntu (not always available) |
| Best For      | Automation, scripting | Manual user creation                 |

### Example:

```bash
sudo useradd -m -s /bin/bash username
sudo adduser username
```
