// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'player_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlayerResponse _$PlayerResponseFromJson(Map<String, dynamic> json) =>
    PlayerResponse(
      id: (json['id'] as num).toInt(),
      songId: (json['songId'] as num).toInt(),
      title: json['title'] as String,
      artistName: json['artistName'] as String,
      duration: (json['duration'] as num).toInt(),
      coverUrl: json['coverUrl'] as String,
      externalTrackId: json['externalTrackId'] as String,
      hasPrevious: json['hasPrevious'] as bool,
      hasNext: json['hasNext'] as bool,
    );

Map<String, dynamic> _$PlayerResponseToJson(PlayerResponse instance) =>
    <String, dynamic>{
      'id': instance.id,
      'songId': instance.songId,
      'title': instance.title,
      'artistName': instance.artistName,
      'duration': instance.duration,
      'coverUrl': instance.coverUrl,
      'externalTrackId': instance.externalTrackId,
      'hasPrevious': instance.hasPrevious,
      'hasNext': instance.hasNext,
    };
