---
type: schema
updated: 2026-07-09
status: active
---

# Wiki Schema

This wiki is maintained by the LLM agent. Obsidian is the human viewer; the markdown files are the durable project memory.

## Domain

- Domain: projectless Codex workspace / knowledge work.
- Main unit of work: work items.
- Main content folder: `wiki/work-items/`.

## Page Templates

### Index Entry

Use one line in `wiki/index.md` for every maintained page:

```markdown
- [[path/to/page]] - one short description.
```

### Work Item

```markdown
---
type: work-item
updated: YYYY-MM-DD
status: planned | active | complete | blocked | abandoned
verdict: undecided | useful | superseded | failed
---

# <Work Item Name>

## Goal

## Context

## Decisions

## Outcome

## Links
```

### Concept

```markdown
---
type: concept
updated: YYYY-MM-DD
status: draft | stable | deprecated
verdict: current | superseded
---

# <Concept>

## Meaning

## Why It Matters

## Examples

## Related Pages
```

### Research

```markdown
---
type: research
updated: YYYY-MM-DD
status: unread | active | summarized | archived
verdict: useful | mixed | not-useful | superseded
---

# <Source or Topic>

## Source Link

## Takeaways

## Caveats

## Used In
```

### Reference

Reference pages route to authoritative pages or files. They do not restate details.

```markdown
---
type: reference
updated: YYYY-MM-DD
status: active | stale
verdict: authoritative-link | needs-review
---

# <Topic>

## Authoritative Source

- [[path/to/source]]

## Related
```

## Workflows

### Before Starting Substantive Work

1. Read `wiki/index.md`.
2. Read `wiki/Failed Ideas/ledger.md`.
3. Read any relevant work item, concept, research, or reference pages linked from the index.
4. If the work risks repeating a failed idea, state the known failure reason before proceeding.

### After Completing a Work Item

1. Create or update a page in `wiki/work-items/` using the work item template.
2. Add a line to `wiki/index.md` if the page is new.
3. Append one dated line to `wiki/log.md`.
4. Update `wiki/lessons.md` if the result changes cross-cutting understanding.
5. Run `python3 wiki/update_hot.py` or rely on the Stop hook.

### After Abandoning an Idea

1. Add or update the relevant work item page with `status: abandoned` and `verdict: failed`.
2. Add one row to `wiki/Failed Ideas/ledger.md` with the failure reason.
3. Append one dated line to `wiki/log.md`.
4. Prefer linking to raw evidence instead of copying it into the wiki.

### Adding External Research

1. Create a page in `wiki/research/` from the research template.
2. Link to the source rather than copying the source text.
3. Add takeaways and caveats in your own words.
4. Add a line to `wiki/index.md`.
5. Append one dated line to `wiki/log.md`.

### Adding Shared Vocabulary

1. Create a page in `wiki/concepts/` from the concept template.
2. Define the term in project-specific language.
3. Link related pages.
4. Add a line to `wiki/index.md`.

## Lint

Periodically check:

- Orphan pages: markdown files under `wiki/` that are not linked from `wiki/index.md` or another page.
- Stale entries: pages marked superseded, deprecated, stale, or archived that still appear current elsewhere.
- Contradictions: pages that give conflicting decisions or definitions.
- Missing concept pages: recurring terms that appear in work items or research but lack pages in `wiki/concepts/`.
- Failed idea coverage: abandoned or reverted work should have a row in `wiki/Failed Ideas/ledger.md`.
- Raw-source boundaries: wiki pages should link to raw sources, code, data, and outputs, not become the source of truth for their contents.
