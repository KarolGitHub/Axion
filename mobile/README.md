# Axion Mobile App

A React Native mobile application for the Axion project management platform, built with Expo.

## Features

- **Cross-platform**: iOS and Android support
- **Modern UI**: Built with React Native Paper
- **Navigation**: Tab-based navigation with React Navigation
- **Authentication**: Login screen with demo mode
- **Dashboard**: Overview with stats and quick actions
- **Calendar Integration**: Native calendar access
- **Offline Support**: Local data storage
- **Push Notifications**: Real-time updates
- **Dark Mode**: Automatic theme switching

## Tech Stack

- **React Native**: 0.73.6
- **Expo**: 50.0.0
- **React Navigation**: 6.x
- **React Native Paper**: 5.12.1
- **TypeScript**: 5.1.3
- **Expo Vector Icons**: 14.0.0
- **React Native Calendars**: 1.1300.0
- **React Native Chart Kit**: 6.12.0

## Prerequisites

- Node.js 18+
- npm or yarn
- Expo CLI (`npm install -g @expo/cli`)
- iOS Simulator (for iOS development)
- Android Studio (for Android development)

## Installation

1. **Install dependencies**:

   ```bash
   cd mobile
   npm install
   ```

2. **Start the development server**:

   ```bash
   npm start
   ```

3. **Run on specific platform**:

   ```bash
   # iOS
   npm run ios

   # Android
   npm run android

   # Web
   npm run web
   ```

## Project Structure

```
mobile/
├── src/
│   ├── components/     # Reusable UI components
│   ├── pages/         # Screen components
│   ├── services/      # API and business logic
│   ├── types/         # TypeScript type definitions
│   └── utils/         # Utility functions
├── assets/            # Images, fonts, etc.
├── App.tsx           # Main app component
├── app.json          # Expo configuration
└── package.json      # Dependencies
```

## Key Features

### Authentication

- Login screen with email/password
- Demo mode for testing
- Secure token storage

### Dashboard

- Overview statistics
- Recent tasks display
- Quick action buttons
- Pull-to-refresh functionality

### Navigation

- Bottom tab navigation
- Stack navigation for screens
- Icon-based navigation

### UI Components

- Material Design components
- Custom styled components
- Responsive layouts
- Dark mode support

## Development

### Adding New Screens

1. Create a new screen component in `src/pages/`
2. Add the route to `App.tsx`
3. Update navigation if needed

### Styling

- Use React Native Paper components
- Follow Material Design guidelines
- Use StyleSheet for custom styles
- Support both light and dark themes

### State Management

- Use React hooks for local state
- Consider Context API for global state
- Implement proper error handling

## Building for Production

### iOS

```bash
expo build:ios
```

### Android

```bash
expo build:android
```

### EAS Build (Recommended)

```bash
# Install EAS CLI
npm install -g @expo/eas-cli

# Configure EAS
eas build:configure

# Build for platforms
eas build --platform ios
eas build --platform android
```

## Configuration

### Environment Variables

Create a `.env` file for environment-specific configuration:

```
API_URL=https://api.axion.com
ENVIRONMENT=development
```

### App Configuration

Update `app.json` for:

- App name and version
- Bundle identifiers
- Permissions
- Plugins

## Testing

### Unit Tests

```bash
npm test
```

### E2E Tests

```bash
npm run test:e2e
```

## Deployment

### App Store (iOS)

1. Build with EAS
2. Submit to App Store Connect
3. Configure app metadata

### Google Play Store (Android)

1. Build with EAS
2. Upload to Google Play Console
3. Configure app metadata

## Contributing

1. Follow the existing code style
2. Add TypeScript types for new features
3. Test on both iOS and Android
4. Update documentation

## License

This project is part of the Axion project management platform.
