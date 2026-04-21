// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'player_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlayerResponse _$PlayerResponseFromJson(Map<String, dynamic> json) =>
    PlayerResponse(
      id: (json['id'] as num).toInt(),
      userId: (json['userId'] as num).toInt(),
      username: json['username'] as String?,
      songId: (json['songId'] as num).toInt(),
      songTitle: json['songTitle'] as String?,
      songCoverUrl: json['songCoverUrl'] as String?,
      previewUrl: json['previewUrl'] as String?,
      currentSeconds: (json['currentSeconds'] as num).toInt(),
      isPlaying: json['isPlaying'] as bool,
      isVisible: json['isVisible'] as bool,
      updatedAt: DateTime.parse(json['updatedAt'] as String),
    );

Map<String, dynamic> _$PlayerResponseToJson(PlayerResponse instance) =>
    <String, dynamic>{
      'id': instance.id,
      'userId': instance.userId,
      'username': instance.username,
      'songId': instance.songId,
      'songTitle': instance.songTitle,
      'songCoverUrl': instance.songCoverUrl,
      'previewUrl': instance.previewUrl,
      'currentSeconds': instance.currentSeconds,
      'isPlaying': instance.isPlaying,
      'isVisible': instance.isVisible,
      'updatedAt': instance.updatedAt.toIso8601String(),
    };
