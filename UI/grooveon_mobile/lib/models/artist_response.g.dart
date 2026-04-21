// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'artist_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Artist _$ArtistResponseFromJson(Map<String, dynamic> json) => Artist(
  id: (json['id'] as num).toInt(),
  name: json['name'] as String,
  pictureUrl: json['pictureUrl'] as String?,
  description: json['description'] as String?,
);

Map<String, dynamic> _$ArtistResponseToJson(Artist instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'pictureUrl': instance.pictureUrl,
  'description': instance.description,
};
