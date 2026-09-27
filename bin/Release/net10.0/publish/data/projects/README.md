# Backup Projects Data

This folder contains backup project data for users in the Fusion Project Manager system.

## File Structure

### Project Metadata
- `{projectid}.json` - Main project information and metadata
- `index.json` - Index of all available projects

### Assignment Files
- `{projectid}a1.json` through `{projectid}a20.json` - Individual assignment data
- Each assignment contains: title, description, status, dates, deliverables, etc.

### Project Phase Files
- `{projectid}p1.json` through `{projectid}p10.json` - Project phase data
- Each phase contains: milestones, budget, team members, deliverables, etc.

## Current Projects

### Project 00000001 - Marketing Campaign (Portia)
**Owner:** portia (userid: 2)
**Status:** Active (35% complete)
**Timeline:** 2025-01-15 to 2025-04-30

**Assignments:**
- ✅ `00000001a1.json` - Market Research & Analysis (100%)
- ✅ `00000001a2.json` - Campaign Strategy Development (100%)
- 🔄 `00000001a3.json` - Creative Asset Development (60%)
- ⏳ `00000001a4.json` - Campaign Launch & Deployment (0%)
- ⏳ `00000001a5.json` - Performance Analysis & Optimization (0%)

**Phases:**
- ✅ `00000001p1.json` - Planning & Strategy Phase (100%)
- 🔄 `00000001p2.json` - Creative Development Phase (60%)
- ⏳ `00000001p3.json` - Launch & Optimization Phase (0%)

## Usage

These backup files can be loaded into the application when:
1. A user logs in and their project data is not available
2. Testing and development purposes
3. Data recovery scenarios
4. Guest mode demonstrations

## Integration

The application's storage helper utilities (`/src/app/utils/storageHelper.ts`) can load these files:
- For regular users: Load into localStorage
- For guest users: Load into sessionStorage
- Fallback when API is unavailable
