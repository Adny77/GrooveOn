import 'package:json_annotation/json_annotation.dart';

part 'genre_stat_item_response.g.dart';

@JsonSerializable()
class GenreStatItemResponse {
  final String genre;
  final int playCount;

  GenreStatItemResponse({
    required this.genre,
    required this.playCount,
  });

  factory GenreStatItemResponse.fromJson(Map<String, dynamic> json) =>
      _$GenreStatItemResponseFromJson(json);

  Map<String, dynamic> toJson() => _$GenreStatItemResponseToJson(this);
}