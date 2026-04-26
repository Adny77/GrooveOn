import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:grooveon_mobile/config/api_config.dart';
import 'package:grooveon_mobile/helper/http_helper.dart';
import 'package:grooveon_mobile/models/music_search_item_response.dart';
import 'package:grooveon_mobile/models/music_search_result.dart';
import 'package:http/http.dart' as http;

class MusicSearchProvider with ChangeNotifier {
  static String baseUrl = '${ApiConfig.apiBase}/api/MusicSearchEngine';

  MusicSearchResult _result = MusicSearchResult();
  bool _isLoading = false;

  MusicSearchResult get result => _result;
  List<MusicSearchItemResponse> get items => _result.items;
  bool get isLoading => _isLoading;

  Future<MusicSearchResult> search({
    String? fts,
    int page = 0,
    int pageSize = 20,
    bool includeTotalCount = true,
    bool retrieveAll = false,
  }) async {
    _isLoading = true;
    notifyListeners();

    try {
      final queryParams = <String, dynamic>{
        'page': page,
        'pageSize': pageSize,
        'includeTotalCount': includeTotalCount.toString(),
        'retrieveAll': retrieveAll.toString(),
      };

      if (fts != null && fts.trim().isNotEmpty) {
        queryParams['fts'] = fts.trim();
      }

      final uri = Uri.parse(baseUrl).replace(
        queryParameters: queryParams.map(
          (key, value) => MapEntry(key, value.toString()),
        ),
      );

      final response = await http.get(
        uri,
        headers: HttpHelper.getHeaders(),
      );

      HttpHelper.checkResponse(response);

      final data = jsonDecode(response.body);
      final parsed = MusicSearchResult.fromJson(data);

      _result = parsed;
      notifyListeners();

      return parsed;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  void clear() {
    _result = MusicSearchResult();
    notifyListeners();
  }
}