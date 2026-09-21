// Swaps the large preview image/caption in the "What we do" section to match
// whichever service row is active, mirroring the hover behaviour in the
// reference screenshots.
document.addEventListener("DOMContentLoaded", () => {
  const rows = document.querySelectorAll(".service-row");
  const preview = document.querySelector(".service-preview img");
  const captionText = document.querySelector(".service-preview__caption p");

  rows.forEach((row) => {
    row.addEventListener("mouseenter", () => {
      rows.forEach((r) => r.classList.remove("is-active"));
      row.classList.add("is-active");

      const image = row.dataset.image;
      if (image && preview) preview.src = image;
    });
  });
});
