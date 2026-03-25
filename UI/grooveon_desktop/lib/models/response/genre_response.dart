import 'package:json_annotation/json_annotation.dart';

part 'genre_response.g.dart';

@JsonSerializable()
class GenreResponse {
  final int id;
  final String externalGenreId;
  final String source;
  final String name;
  final DateTime? createdAt;

  GenreResponse({
    required this.id,
    required this.externalGenreId,
    required this.source,
    required this.name,
    this.createdAt,
  });

  factory GenreResponse.fromJson(Map<String, dynamic> json) =>
      _$GenreResponseFromJson(json);

  Map<String, dynamic> toJson() => _$GenreResponseToJson(this);
}