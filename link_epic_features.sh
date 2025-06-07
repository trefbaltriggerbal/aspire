#!/bin/bash
TOKEN="${GITHUB_TOKEN:?GITHUB_TOKEN not set}"
OWNER="trefbaltriggerbal"
REPO="aspire"

function node_id() {
  curl -s -H "Authorization: Bearer $TOKEN" -H "Accept: application/vnd.github+json" "https://api.github.com/repos/$OWNER/$REPO/issues/$1" | jq -r '.node_id'
}

function link_issue() {
  parent_id=$1
  child_id=$2
  curl -s -X POST -H "Authorization: bearer $TOKEN" -H "Content-Type: application/json" --data '{"query":"mutation AddSub{addSubIssue(input:{issueId:\"'$parent_id'\", subIssueId:\"'$child_id'\"}) { issue {number} subIssue {number} }}"}' https://api.github.com/graphql
  echo
}

# Epic 73: Authenticatie & Security -> features 38 39 40
parent=$(node_id 73)
for f in 38 39 40; do link_issue "$parent" "$(node_id $f)"; done

# Epic 74: Client Functionaliteiten -> 41 42 43 44
parent=$(node_id 74)
for f in 41 42 43 44; do link_issue "$parent" "$(node_id $f)"; done

# Epic 75: Parent Functionaliteiten -> 45 46 47
parent=$(node_id 75)
for f in 45 46 47; do link_issue "$parent" "$(node_id $f)"; done

# Epic 76: Administrator Functionaliteiten -> 48 49 50 51
parent=$(node_id 76)
for f in 48 49 50 51; do link_issue "$parent" "$(node_id $f)"; done

# Epic 77: Documentbeheer -> 52 53 54
parent=$(node_id 77)
for f in 52 53 54; do link_issue "$parent" "$(node_id $f)"; done

# Epic 78: Notificatiesysteem -> 55 56 57
parent=$(node_id 78)
for f in 55 56 57; do link_issue "$parent" "$(node_id $f)"; done

# Epic 79: Meertaligheid -> 58 59 60 61
parent=$(node_id 79)
for f in 58 59 60 61; do link_issue "$parent" "$(node_id $f)"; done

# Epic 80: Logging & Auditing -> 62 63 64
parent=$(node_id 80)
for f in 62 63 64; do link_issue "$parent" "$(node_id $f)"; done

# Epic 81: Performance & Hosting -> 65 66 67 68
parent=$(node_id 81)
for f in 65 66 67 68; do link_issue "$parent" "$(node_id $f)"; done

# Epic 82: Facturatie en betalingen -> 69 70 71 72
parent=$(node_id 82)
for f in 69 70 71 72; do link_issue "$parent" "$(node_id $f)"; done

