import 'package:json_annotation/json_annotation.dart';

part 'genre_upsert_request.g.dart';

@JsonSerializable()
class GenreUpsertRequest {
  final String externalGenreId;
  final String source;
  final String name;

  GenreUpsertRequest({
    required this.externalGenreId,
    this.source = "Deezer",
    required this.name,
  });

  factory GenreUpsertRequest.fromJson(Map<String, dynamic> json) =>
      _$GenreUpsertRequestFromJson(json);

  Map<String, dynamic> toJson() => _$GenreUpsertRequestToJson(this);
}