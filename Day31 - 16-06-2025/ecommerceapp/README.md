# Concepts Learned

| Concept                    | Purpose                                                               |
| -------------------------- | --------------------------------------------------------------------- |
| **RxJS `Subject`**         | Triggers search events whenever user types in the input box.          |
| **`debounceTime`**         | Waits for user to stop typing (400ms) before making API request.      |
| **`distinctUntilChanged`** | Avoids duplicate API calls when search term hasn’t changed.           |
| **`switchMap`**            | Cancels old API requests if new input comes in quickly.               |
| **`HttpClient`**           | Makes HTTP GET requests to fetch search results from backend API.     |
| **`HostListener`**         | Detects when user scrolls near the bottom of the page.                |
| **Infinite Scroll**        | Loads more products automatically as user scrolls down.               |
| **`ngIf`, `ngFor`**        | Angular structural directives to show/hide and loop through products. |


# Task

- Create a simple Angular application that lets users browse and search for products using infinite scroll and debounce-based search. The app will have basic routing with two routes: Home and About.
