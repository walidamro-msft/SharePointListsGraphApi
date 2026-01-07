# Contributing to SharePoint List Management Tool

Thank you for your interest in contributing to this project! 

## How to Contribute

### Reporting Issues

If you find a bug or have a feature request:

1. Check if the issue already exists in the [Issues](https://github.com/walidamro-msft/SharePointListsGraphApi/issues) section
2. If not, create a new issue with:
   - Clear description of the problem or feature
   - Steps to reproduce (for bugs)
   - Expected vs actual behavior
   - Your environment (.NET version, OS, etc.)

### Submitting Changes

1. **Fork the repository**
```bash
git clone https://github.com/walidamro-msft/SharePointListsGraphApi.git
cd SharePointListsGraphApi
```

2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes**
   - Follow the existing code style
   - Add comments where necessary
   - Update documentation if needed

4. **Test your changes**
   ```bash
   dotnet build
   dotnet run
   ```

5. **Commit your changes**
   ```bash
   git add .
   git commit -m "Description of your changes"
   ```

6. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

7. **Create a Pull Request**
   - Go to the original repository
   - Click "New Pull Request"
   - Select your branch
   - Describe your changes

## Development Guidelines

### Code Style

- Follow C# naming conventions
- Use meaningful variable and method names
- Keep methods focused (Single Responsibility Principle)
- Add XML documentation comments for public methods
- Use async/await for I/O operations

### Project Structure

```
SharePointListsGraphApi/
??? Services/           # Business logic
??? Helpers/            # Utility classes
??? Program.cs          # Main entry point
??? SharePointSettings.cs  # Configuration model
```

### Adding New Features

1. Create a new service in the `Services/` folder if needed
2. Update the menu in `MenuHelper.cs`
3. Add handler in `Program.cs`
4. Update documentation (README, USER-GUIDE, etc.)

### Documentation

When adding features, update:
- `README.md` - Overview and quick start
- `USER-GUIDE.md` - Detailed usage instructions
- `ARCHITECTURE.md` - Code structure (if applicable)
- `TROUBLESHOOTING.md` - Common issues (if applicable)

## Testing

Before submitting a PR:

1. **Build the project**
   ```bash
   dotnet build
   ```
   - Ensure no build errors or warnings

2. **Test manually**
   - Test all existing features still work
   - Test your new feature works as expected
   - Test edge cases and error handling

3. **Verify documentation**
   - Check that all documentation is up-to-date
   - Verify code examples work

## Code of Conduct

- Be respectful and constructive
- Welcome newcomers and help them learn
- Focus on what is best for the community
- Show empathy towards others

## Questions?

If you have questions about contributing:
- Check existing issues and pull requests
- Create a new issue with your question
- Review the documentation in `ARCHITECTURE.md`

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing! ??
