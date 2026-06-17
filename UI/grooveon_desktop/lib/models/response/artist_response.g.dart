// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'artist_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ArtistResponse _$ArtistResponseFromJson(Map<String, dynamic> json) =>
    ArtistResponse(
      id: (json['id'] as num).toInt(),
      externalArtistId: json['externalArtistId'] as String?,
      source: json['source'] as String,
      name: json['name'] as String,
      picture: json['picture'] as String?,
      createdAt: DateTime.parse(json['createdAt'] as String),
    );

Map<String, dynamic> _$ArtistResponseToJson(ArtistResponse instance) =>
    <String, dynamic>{
      'id': instance.id,
      'externalArtistId': instance.externalArtistId,
      'source': instance.source,
      'name': instance.name,
      'picture': instance.picture,
      'createdAt': instance.createdAt.toIso8601String(),
    };
