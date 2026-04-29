import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/helper/http_helper.dart';
import 'package:grooveon_desktop/models/response/search_result.dart';
import 'package:http/http.dart' as http;

abstract class BaseProvider<T> with ChangeNotifier {
  final String endpoint;

  BaseProvider(this.endpoint);

  Future<SearchResult<T>> get({Map<String, dynamic>? filter}) async {
    String url = "${ApiConfig.apiBase}/api/$endpoint";

    if (filter != null && filter.isNotEmpty) {
      final query = _buildQuery(filter);
      url = "$url?$query";
    }

    final response = await http.get(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    final json = jsonDecode(response.body);
    final result = SearchResult<T>();

    if (json is Map && json.containsKey("items")) {
      result.totalCount = json["totalCount"] ?? json["items"].length;

      for (var item in json["items"]) {
        result.items.add(fromJson(item));
      }
    } else if (json is List) {
      result.totalCount = json.length;

      for (var item in json) {
        result.items.add(fromJson(item));
      }
    }

    return result;
  }

  Future<T> getById(int id) async {
    final url = "${ApiConfig.apiBase}/api/$endpoint/$id";

    final response = await http.get(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    return fromJson(jsonDecode(response.body));
  }

  Future<T> insert(dynamic request) async {
    final url = "${ApiConfig.apiBase}/api/$endpoint";

    final response = await http.post(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request),
    );

    HttpHelper.checkResponse(response);

    return fromJson(jsonDecode(response.body));
  }

  Future<T> update(int id, dynamic request) async {
    final url = "${ApiConfig.apiBase}/api/$endpoint/$id";

    final response = await http.put(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request),
    );

    HttpHelper.checkResponse(response);

    return fromJson(jsonDecode(response.body));
  }

  Future<bool> delete(int? id) async {
    final url = "${ApiConfig.apiBase}/api/$endpoint/$id";

    final response = await http.delete(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    return true;
  }

  T fromJson(dynamic data);

  String _buildQuery(Map<String, dynamic> params) {
    return params.entries
        .map(
          (e) =>
              "${Uri.encodeComponent(e.key)}=${Uri.encodeComponent(e.value.toString())}",
        )
        .join("&");
  }
}