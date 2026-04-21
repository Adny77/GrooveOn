import 'package:json_annotation/json_annotation.dart';

part 'player_response.g.dart';

@JsonSerializable()
class PlayerResponse {
  final int id;

  final int userId;
  final String? username;

  final int songId;
  final String? songTitle;
  final String? songCoverUrl;
  final String? previewUrl;

  final int currentSeconds;
  final bool isPlaying;
  final bool isVisible;

  final DateTime updatedAt;

  PlayerResponse({
    required this.id,
    required this.userId,
    this.username,
    required this.songId,
    this.songTitle,
    this.songCoverUrl,
    this.previewUrl,
    required this.currentSeconds,
    required this.isPlaying,
    required this.isVisible,
    required this.updatedAt,
  });

  factory PlayerResponse.fromJson(Map<String, dynamic> json) =>
      _$PlayerResponseFromJson(json);

  Map<String, dynamic> toJson() => _$PlayerResponseToJson(this);
}