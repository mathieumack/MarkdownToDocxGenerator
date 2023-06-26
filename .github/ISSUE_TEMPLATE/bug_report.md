---
name: Bug report
about: Create a report to help us improve
title: ''
labels: bug
assignees: ''

---

**Describe the bug**
A clear and concise description of what the bug is.

**To Reproduce**

Steps to reproduce the behavior.
Ideally, describes :
* Json input:
```json
{
    "resultField1": 132.46,
    "resultField2": true
}
```
* Json transformation:
```json
{
    "resultField1": "$.field1->ToInteger()",
    "resultField2": "$.field2"
}
```
* Json result:
```json
{
    "resultField1": "hello",
    "resultField2": ""
}
```
* Json expected result:
```json
{
    "resultField1": 132,
    "resultField2": true
}
```

**Expected behavior**
A clear and concise description of what you expected to happen.

**Additional context**
Add any other context about the problem here.
