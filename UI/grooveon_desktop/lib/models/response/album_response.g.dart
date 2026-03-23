// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumResponse _$AlbumResponseFromJson(Map<String, dynamic> json) =>
    AlbumResponse(
      id: (json['id'] as num).toInt(),
      externalAlbumId: json['externalAlbumId'] as String?,
      source: json['source'] as String,
      title: json['title'] as String,
      artistId: (json['artistId'] as num).toInt(),
      artistName: json['artistName'] as String,
      releaseDate: json['releaseDate'] == null
          ? null
          : DateTime.parse(json['releaseDate'] as String),
      coverUrl: json['coverUrl'] as String?,
      description: json['description'] as String?,
      createdAt: DateTime.parse(json['createdAt'] as String),
      songCount: (json['songCount'] as num).toInt(),
    );

Map<String, dynamic> _$AlbumResponseToJson(AlbumResponse instance) =>
    <String, dynamic>{
      'id': instance.id,
      'externalAlbumId': instance.externalAlbumId,
      'source': instance.source,
      'title': instance.title,
      'artistId': instance.artistId,
      'artistName': instance.artistName,
      'releaseDate': instance.releaseDate?.toIso8601String(),
      'coverUrl': instance.coverUrl,
      'description': instance.description,
      'createdAt': instance.createdAt.toIso8601String(),
      'songCount': instance.songCount,
    };
