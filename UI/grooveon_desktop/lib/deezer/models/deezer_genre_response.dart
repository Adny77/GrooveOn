import 'package:json_annotation/json_annotation.dart';

part 'deezer_genre_response.g.dart';

@JsonSerializable()
class DeezerGenreResponse {
  final String id;
  final String name;

  DeezerGenreResponse({
    required this.id,
    required this.name,
  });

  factory DeezerGenreResponse.fromJson(Map<String, dynamic> json) =>
      _$DeezerGenreResponseFromJson({
        ...json,
        "id": json["id"].toString(),
      });

  Map<String, dynamic> toJson() => _$DeezerGenreResponseToJson(this);
}