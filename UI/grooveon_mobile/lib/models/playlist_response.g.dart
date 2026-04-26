// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'playlist_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlaylistResponse _$PlaylistResponseFromJson(Map<String, dynamic> json) =>
    PlaylistResponse(
      id: (json['id'] as num).toInt(),
      userId: (json['userId'] as num).toInt(),
      username: json['username'] as String?,
      name: json['name'] as String,
      description: json['description'] as String?,
      isPublic: json['isPublic'] as bool,
      coverImageUrl: json['coverImageUrl'] as String?,
      createdAt: DateTime.parse(json['createdAt'] as String),
      songCount: (json['songCount'] as num).toInt(),
    );

Map<String, dynamic> _$PlaylistResponseToJson(PlaylistResponse instance) =>
    <String, dynamic>{
      'id': instance.id,
      'userId': instance.userId,
      'username': instance.username,
      'name': instance.name,
      'description': instance.description,
      'isPublic': instance.isPublic,
      'coverImageUrl': instance.coverImageUrl,
      'createdAt': instance.createdAt.toIso8601String(),
      'songCount': instance.songCount,
    };
