import 'package:grooveon_desktop/exception/ApiException.dart';
import 'package:grooveon_desktop/helper/exception_read_helper.dart';
import 'package:grooveon_desktop/utils/session.dart';
import 'package:http/http.dart' as http;
class HttpHelper {
  static void checkResponse(http.Response response) {
  if (response.statusCode >= 200 && response.statusCode < 300) {
    return;
  }

  String message = extractApiErrorMessage(response.body);

  if (response.statusCode == 401) {
    throw ApiException(message.isNotEmpty
        ? message
        : "NEAUTORIZOVAN");
  }

  if (response.statusCode == 403) {
    throw ApiException(message.isNotEmpty
        ? message
        : "ZABRANJENO");
  }

  if (response.statusCode == 404) {
    throw ApiException(message.isNotEmpty
        ? message
        : "NOTFOUND");
  }

  throw ApiException(message);
}

  static Map<String, String> getHeaders({bool withToken = true}) {
  final headers = {
    "Content-Type": "application/json",
    "Accept": "application/json",
  };

  if (withToken && Session.token != null) {
    headers["Authorization"] = "Bearer ${Session.token}";
  }

  return headers;
}
}