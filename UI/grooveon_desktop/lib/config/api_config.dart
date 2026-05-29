class ApiConfig {
  static const String apiBase = String.fromEnvironment(
    'API_BASE_URL_DESKTOP',
    defaultValue: 'http://localhost:5277',
  );
}
