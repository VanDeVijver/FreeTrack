# SAPstudios — site analysis

Source: `https://sap-studio-bert.on.adaptive.ai/` (blocked to automated fetch by
robots.txt — this is reconstructed from the 3 screenshots and the page's
view-source you provided).

## What it's built with
Confirmed from `view-source`:
- **React + Vite** dev build (`/@vite/client`, `/@react-refresh`, `/src/main.tsx`) —
  this is a client-side rendered single-page app, not server-rendered HTML.
- Hash-based routing / in-page anchors: `#top`, `#diensten`.
- Fonts loaded via Google Fonts: **DM Sans** (400/500/600/700) for body text,
  **Space Grotesk** (400/500/600/700) for headings/UI.
- `theme-color` meta is `#151414` (near-black), matching the dark sections.
- Cloudflare Web Analytics beacon script.
- Language: Dutch (`lang="nl"`), copy mixes Dutch body text with English
  micro-copy ("EXPLORE SERVICE", "Contact Us").

## Style guide

### Color palette (approximate, read from screenshots)
| Role | Color | Notes |
|---|---|---|
| Light background | `#EDEAE5` / off-white bone | Hero + rooms section background |
| Dark background | `#151414` / near-black | "What we do" section, matches `theme-color` |
| Primary accent | `#E17B52` burnt orange/coral | Headline accent word, buttons, CTA band |
| Secondary accent | `#C7D65A` yellow-green | Small kicker labels on dark sections ("WHAT WE DO"), arrow icons |
| Text on light | near-black `#1A1A1A` | Headings |
| Text on dark | white / warm gray | Headings + body on dark section |
| Muted label | gray `#8A8A85` | Small tracked-out kicker text on light sections |

### Typography
- **Space Grotesk** — large display headings ("Sound is a *place.*", "Gemaakt
  voor *goede ideeën.*", "Find your *frequency.*"). Bold, tight leading, big
  size jump between the plain word and the accented word.
- The accented word in each headline ("place.", "goede ideeën.", "frequency.")
  is rendered in the orange accent color with an **italic serif-leaning**
  treatment — visually distinct from the rest of the heading. This look isn't
  achievable with Space Grotesk alone, so it's either a second display font
  loaded elsewhere (not in the `<head>` you captured) or a heavily styled
  italic variant. Worth checking the built CSS bundle to confirm.
- **DM Sans** — body copy, nav links, small labels, buttons.
- Kicker labels (`SAP STUDIOS / WETTEREN`, `WHAT WE DO`, `THE ROOMS`) are
  small, uppercase, letter-spaced.

### Layout & components
- **Nav bar**: logo + tagline left ("SAP STUDIOS / SONIC ADVENTURE
  PRODUCTIONS"), horizontal link list right (HOME, DIENSTEN, STUDIO'S &
  LIGGING, HUUR MIJ, OVER SAP), active link underlined.
- **Hero**: split layout — left column is text (kicker, big heading, intro
  paragraph, primary + secondary CTA, a "01 —— 04" slide/progress indicator),
  right column is a full-bleed photo. A thin ticker/marquee bar sits under the
  hero with rotating labels ("LISTEN CLOSELY", "MAKE IT LOUD", "STAY CURIOUS",
  "FROM THE WOODS") separated by a small asterisk glyph.
- **Floating right-side icon rail**: small dark circular buttons stacked
  vertically along the right edge, present on every section (looks like quick
  links or a persistent utility menu).
- **Services section ("Diensten")**: dark background. Left column is a
  numbered list (01/02/03) of services, each row has a small category label,
  a bold title, an arrow icon, and a horizontal divider; rows appear to be
  interactive/expandable (hover state shown on row 01 vs. muted 02/03). Right
  column shows a large photo tied to the hovered/active row, with a caption
  bottom-left and an "EXPLORE SERVICE →" link bottom-right.
- **Rooms grid ("Find your frequency")**: light background, 3 asymmetric
  cards (top-left corner rounded, others square) each with a background photo,
  a color-tinted overlay (blue-gray, amber, red — one tint per room), a
  number + badge label top row, room name + spec line + short description
  bottom-left, and a circular arrow button bottom-right. The third card
  (LOVIT) shows a "Contact Us" pill button centered on hover/active state
  instead of just the arrow. A "Bekijk alle mogelijkheden →" link sits under
  the grid.
- **Full-width orange band**: appears between sections, likely a CTA or
  transition/footer teaser.
- Small decorative details: geo-coordinates text (`50°59'N / 03°52'E`), thin
  1px dividers, scroll-to-top arrow (`↑`).

## Functional inventory (inferred)
This is a **marketing / lead-generation site for a recording studio**
(SAPstudios, Wetteren, Belgium), not an e-commerce or self-service booking
engine. No cart, checkout, or account system is visible anywhere.

- **Home** — hero pitch + section teasers, scroll/slide progress indicator.
- **Diensten (Services)** — 3 services: *Live & op locatie*, *Studio &
  productie*, *Workshops & sessies*, each with its own description and an
  "explore" link (likely scrolls/routes to a dedicated detail view).
- **Studio's & ligging (Rooms & location)** — 3 rooms: *Studio Lively* (45m²
  liveroom), *Studio Poly* (55m² flexible/polyvalent room), *LOVIT* (100m²
  loft with kitchenette). Each card links out to more detail or straight to
  contact.
- **Huur mij (Rent me)** — presumably a booking/availability or pricing page
  for renting a room.
- **Over SAP (About)** — studio bio.
- **Contact / inquiry** — two entry points seen: "Vertel ons over je
  project →" (hero) and "Contact Us" (LOVIT card). Almost certainly a single
  contact form (name, email, message, maybe preferred room/service/date)
  rather than a live calendar booking system — nothing in the screenshots
  suggests real-time availability or payment.

## What this means for a backend
The content is fully static/hardcoded in the current React build (no fetch
calls visible in the head, all copy is baked into JSX). The natural backend
role isn't "power an existing dynamic app" — it's:

1. Turn the hardcoded rooms/services/copy into **editable content** (so
   non-developers can update room descriptions, prices, photos).
2. Give the two contact CTAs a real **endpoint** to submit to, with storage
   and (later) email notification.

That's what the scaffolded API below is built around — `Rooms`,
`StudioServices`, and `ContactInquiries` are exactly the three things this
site actually needs a database for.
