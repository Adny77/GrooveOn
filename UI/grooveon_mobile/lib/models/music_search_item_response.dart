import 'package:json_annotation/json_annotation.dart';

part 'music_search_item_response.g.dart';

@JsonSerializable()
class MusicSearchItemResponse {
  final String type;
  final int id;
  final String title;
  final String? subtitle;
  final String? imageUrl;
  final String? previewUrl;
  final int? artistId;
  final int? albumId;

  String? externalTrackId; 

  MusicSearchItemResponse({
    required this.type,
    required this.id,
    required this.title,
    this.subtitle,
    this.imageUrl,
    this.previewUrl,
    this.artistId,
    this.albumId,
    this.externalTrackId
  });

  factory MusicSearchItemResponse.fromJson(Map<String, dynamic> json) =>
      _$MusicSearchItemResponseFromJson(json);

  Map<String, dynamic> toJson() => _$MusicSearchItemResponseToJson(this);
}