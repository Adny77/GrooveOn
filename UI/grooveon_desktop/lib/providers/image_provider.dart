import 'dart:convert';
import 'dart:io';

import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/helper/http_helper.dart';
import 'package:http/http.dart' as http;
import 'package:path/path.dart' as p;

class ImageAppProvider {
  ImageAppProvider._();

  static Future<String> upload({
    required File file,
    required String folder,
  }) async {
    final uri = Uri.parse(
      '${ApiConfig.apiBase}/api/images/upload?folder=$folder',
    );

    final request = http.MultipartRequest('POST', uri);

    request.headers.addAll(HttpHelper.getHeaders());

    request.files.add(
      await http.MultipartFile.fromPath(
        'file',
        file.path,
        filename: p.basename(file.path),
      ),
    );

    final streamed = await request.send();
    final response = await http.Response.fromStream(streamed);

    HttpHelper.checkResponse(response);

    final json = jsonDecode(response.body);
    return json['fileName'];
  }

  static Future<void> delete({
    required String folder,
    required String fileName,
  }) async {
    final uri = Uri.parse(
      '${ApiConfig.apiBase}/api/images'
      '?folder=$folder&fileName=$fileName',
    );

    final res = await http.delete(
      uri,
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(res);
  }
}