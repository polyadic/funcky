# Migrating from 2.x to 3.0

1. Update to the latest 2.x version of Funcky (2.7.1) and fix all warnings.
2. Update to Funcky 3.0.
3. Run `dotnet format analyzers`. This will migrate `Option<T>.None()` to `Option<T>.None` for you. \
   This command sometimes fails while loading your project(s). `--no-restore` might help with that.
4. Build and fix other compilation failures.
5. You might need to re-run `dotnet format analyzers` after fixing other errors.
6. **Important:** Check if you're using `System.Text.Json` to serialize `Option`s.
   This will no longer work automatically. You need to add the `OptionJsonConverter` yourself.
