// Swaps the large preview image/caption in the "What we do" section to match
// whichever service row is active (hover or keyboard focus), mirroring the
// hover behaviour in the reference screenshots.
document.addEventListener("DOMContentLoaded", () => {
  const rows = document.querySelectorAll(".service-row");
  const preview = document.querySelector(".service-preview img");
  const captionText = document.querySelector(".service-preview__caption p");

  const activate = (row) => {
    rows.forEach((r) => r.classList.remove("is-active"));
    row.classList.add("is-active");

    const { image, description } = row.dataset;
    if (image && preview) preview.src = image;
    if (description && captionText) captionText.textContent = description;
  };

  rows.forEach((row) => {
    row.addEventListener("mouseenter", () => activate(row));
    row.addEventListener("focus", () => activate(row));
  });
});
