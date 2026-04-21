import 'package:json_annotation/json_annotation.dart';

part 'music_stat_item_response.g.dart';

@JsonSerializable()
class MusicStatItemResponse {
  final int id;
  final String title;
  final String? imageUrl;
  final int playCount;

  MusicStatItemResponse({
    required this.id,
    required this.title,
    this.imageUrl,
    required this.playCount,
  });

  factory MusicStatItemResponse.fromJson(Map<String, dynamic> json) =>
      _$MusicStatItemResponseFromJson(json);

  Map<String, dynamic> toJson() => _$MusicStatItemResponseToJson(this);
}