import 'package:json_annotation/json_annotation.dart';

part 'playlist_response.g.dart';

@JsonSerializable()
class PlaylistResponse {
  final int id;

  final int userId;
  final String? username;

  final String name;
  final String? description;

  final bool isPublic;
  final String? coverImageUrl;

  final DateTime createdAt;

  final int songCount;

  PlaylistResponse({
    required this.id,
    required this.userId,
    this.username,
    required this.name,
    this.description,
    required this.isPublic,
    this.coverImageUrl,
    required this.createdAt,
    required this.songCount,
  });

  factory PlaylistResponse.fromJson(Map<String, dynamic> json) =>
      _$PlaylistResponseFromJson(json);

  Map<String, dynamic> toJson() => _$PlaylistResponseToJson(this);
}